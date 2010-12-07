Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Threading
Imports System.Management
Imports System.xml
Imports System.Xml.XPath


Public Class form_Migration

    ' Set up the USB Event watcher
    Private WithEvents eventwatcher_USBStateChange As ManagementEventWatcher
    ' Set up the classes with events
    Private WithEvents class_Migration As classMigration
    Private WithEvents class_HealthCheck As classHealthCheck

#Region "Form Events"

    ' Form Loading
    Private Sub form_MigrationLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        appInitialise()

        sub_DebugMessage()
        sub_DebugMessage("* Form Startup Events *")

        ' *** Set up form options
        sub_DebugMessage("Setting form defaults...")

        Me.Text = My.Resources.appTitle & " " & My.Resources.appBuild

        ' Set the default Migration option
        str_MigrationType = "SCANSTATE"

        ' Check the OS Architecture and set options accordingly
        Select Case str_OSArchitecture
            Case "x86"
                str_USMTGUID = str_USMTGUIDx86
            Case "x64"
                str_USMTGUID = str_USMTGUIDx64
        End Select

        ' Check the OS version and set options accordingly
        If dbl_OSVersion >= 6.0 And bln_MigrationXPOnly Then
            bln_MigrationXPOnly = False
            sub_DebugMessage("WARNING: XP Only Mode is specified but OS is not XP! Disabling...")
        End If

        ' Get OS Information and display on the form
        label_OSVersion.Text = str_OSFullName

        ' Set Multi User Mode if desired
        If bln_MigrationMultiUserMode And Not radiobox_WorkstationDetails2.Checked Then
            radiobox_WorkstationDetails2.Checked = True
        End If

        ' If Workstation Details is disabled through settings, disable the form controls
        If bln_SettingsWorkstationDetailsDisabled Then
            slabel_WorkstationDetails.Enabled = False
            radiobox_WorkstationDetails1.Enabled = False
            radiobox_WorkstationDetails2.Enabled = False
        End If

        ' If Health Check is enabled through settings, enable on the form controls
        If bln_SettingsHealthCheckDefaultEnabled Then
            checkbox_HealthCheck.Checked = True
            bln_HealthCheck = False
        End If

        ' If Advanced Settings is disabled through settings, disable the form controls
        If bln_SettingsAdvancedSettingsDisabled Then
            button_AdvancedSettings.Visible = False
        Else
            button_AdvancedSettings.Visible = True
        End If

        ' Reset status labels
        label_MigrationCurrentPhase.Text = "..."
        label_MigrationEstSize.Text = "..."
        label_MigrationEstTimeRemaining.Text = "..."

        ' Check for USB devices and initialise USB Event watcher
        sub_USBInitialise()

        ' Check the command-line for actioning
        sub_InitParseCommandLine()

    End Sub

    ' Form Showing
    Private Sub form_MigrationShown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown

        sub_DebugMessage()
        sub_DebugMessage("* Form Shown Events *")

        ' Check if the Key is encrypted in the settings file
        'sub_InitCheckEncryption()

        ' Check if USMT is installed
        sub_InitCheckForUSMT()

        ' Check if Config files exist
        sub_InitCheckForRuleSetFiles()

        ' Check if the WMA configuration is valid
        If Not func_InitCheckForValidConfiguration() Then
            sub_DebugMessage("WARNING: It looks like you haven't yet modified the WMA configuration file (MigAssistant.Exe.Config). Your migration may fail as a result!", True)
            ' If we're running in Progress Only mode, start the migration immediately
        ElseIf bln_AppProgressOnlyMode Then
            sub_DebugMessage("Progress Only Mode. Starting Migration...")
            button_Start.PerformClick()
        End If

    End Sub

    ' Form Closing
    Private Sub form_MigrationClosing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

        sub_DebugMessage()
        sub_DebugMessage("* Form Closing Events *")

        'Exit the application
        appShutdown(My.Resources.exitCodeOk)

    End Sub

    ' Tab Control
    Private Sub tabcontrol_MigrationType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabcontrol_MigrationType.SelectedIndexChanged, tabcontrol_MigrationType.DrawItem

        sub_DebugMessage()
        sub_DebugMessage("* Tab Change / Drawn Events * ")

        ' Advanced / Back / Forward button setup
        Select Case tabcontrol_MigrationType.SelectedIndex

            Case tabcontrol_MigrationType.TabPages.IndexOf(tabpage_Capture)

                sub_DebugMessage("Capture Tab Selected")
                str_MigrationType = "SCANSTATE"

            Case tabcontrol_MigrationType.TabPages.IndexOf(tabpage_Restore)

                sub_DebugMessage("Restore Tab Selected")
                str_MigrationType = "LOADSTATE"

                label_DatastoreLocation.Text = "Searching..."
                Application.DoEvents()

                sub_MigrationFindDataStore()

        End Select

        button_Start.Focus()

    End Sub

    ' Handle Start / Stop Buttons
    Private Sub button_StartStopClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_Start.Click

        sub_DebugMessage()
        sub_DebugMessage("* Start / Stop Button Click Events *")

        If Not bln_HealthCheckInProgress And Not bln_MigrationInProgress Then
            sub_DebugMessage("No actions currently in progress")
            sub_MigrationInitialise()
        Else
            If bln_HealthCheckInProgress Then
                sub_DebugMessage("Health Check currently in progress")
                class_HealthCheck.SpinDown()
            End If
            If bln_MigrationInProgress Then
                sub_DebugMessage("Migration currently in progress")
                class_Migration.SpinDown()
            End If
        End If

    End Sub

    ' Data Capture - Workstation Details Radioboxes
    Private Sub radiobox_WorkstationDetails_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles radiobox_WorkstationDetails1.CheckedChanged, radiobox_WorkstationDetails2.CheckedChanged

        sub_DebugMessage()
        sub_DebugMessage("* Workstation Details Radiobox Change Events *")

        If radiobox_WorkstationDetails2.Checked Then
            sub_DebugMessage("Switched to migrate All Users")
            bln_MigrationSettingsAllUsers = True
        Else
            sub_DebugMessage("Switched to migrate Current User")
            bln_MigrationSettingsAllUsers = False
        End If

    End Sub

    ' Data Capture - Health Check Skip Checkbox
    Private Sub checkbox_HealthCheck_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles checkbox_HealthCheck.CheckedChanged

        sub_DebugMessage()
        sub_DebugMessage("* Health Check Checkbox Events *")

        If checkbox_HealthCheck.Checked Then
            sub_DebugMessage("Health Check Enabled")
            bln_HealthCheck = True
        Else
            sub_DebugMessage("Health Check Disabled")
            bln_HealthCheck = False
        End If

    End Sub

    ' Data Capture - Advanced Settings
    Private Sub btnMigrationSettingsAdvanced_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button_AdvancedSettings.Click

        sub_DebugMessage()
        sub_DebugMessage("* Advanced Settings Button Events *")

        formMigrationAdvancedSettings.Show()
        button_Start.Focus()

    End Sub

#End Region

#Region "Initialisation"

    'Private Sub sub_InitCheckEncryption()

    '    sub_DebugMessage()
    '    sub_DebugMessage("* Check Encryption *")

    '    ' If encryption is disabled, skip
    '    If bln_MigrationEncryptionDisabled Then

    '        sub_DebugMessage("Encryption is disabled. Skipping...")
    '        Exit Sub
    '    End If

    '    ' If the key is not already encrypted
    '    If Not bln_MigrationEncryptionDefaultKeyEncrypted Then

    '        sub_DebugMessage("Key is not previously encrypted. Encrypting...")

    '        ' Encrypt Key
    '        Dim encryption_EncryptedData As Encryption.Data = _
    '            encryption_SymmetricEncryption.Encrypt(New Encryption.Data(str_MigrationEncryptionDefaultKey), encryption_DataHash)

    '        My.Settings.MigrationEncryptionDefaultKey = encryption_EncryptedData.Text
    '        My.Settings.MigrationEncryptionDefaultKeyEncrypted = True
    '        My.Settings.Save()

    '        MsgBox("Encrypted: " & encryption_EncryptedData.Text)

    '        ' Decrypt Key
    '        Dim encryption_DecryptedData As Encryption.Data = _
    '            encryption_SymmetricEncryption.Decrypt(encryption_EncryptedData, encryption_DataHash)

    '        MsgBox("Decrypted: " & encryption_DecryptedData.Text)

    '    End If

    'End Sub

    Private Sub sub_InitCheckForUSMT()

        sub_DebugMessage()
        sub_DebugMessage("* Check for USMT *")

        Dim bln_SkipOnlineCheck As Boolean
        Dim bln_IsUSMTInstalled As Boolean = True
        Dim str_bddManifestFileFullPath As String = Path.Combine(str_TempFolder, str_bddManifestFile)
        Dim str_bddComponentFileFullPath As String = Path.Combine(str_TempFolder, My.Resources.bddComponentFile)
        Dim str_USMTSearchFile As String = "ScanState.Exe"
        Dim xml_GUID As String = Nothing
        Dim xml_Architecture As String = Nothing
        Dim xml_InstallCondition As String = Nothing
        Dim xml_DownloadURL As String = Nothing
        Dim xml_DownloadFile As String = Nothing
        Dim xml_Description As String = Nothing

        ' Check for USMT in the relevant folders
        sub_DebugMessage("Checking for USMT file: " & str_USMTSearchFile & "...")
        Try
            If Not My.Computer.FileSystem.FileExists(Path.Combine(str_WMAFolder, str_USMTSearchFile)) Then
                ' If not found, search the Program Files folder for any matches
                For Each foundDirectory As String In My.Computer.FileSystem.GetDirectories(My.Computer.FileSystem.SpecialDirectories.ProgramFiles, FileIO.SearchOption.SearchTopLevelOnly, "USMT*")
                    str_USMTFolder = foundDirectory
                    bln_IsUSMTInstalled = True
                Next
                ' If still no USMT found, exit the application with an error
                If Not My.Computer.FileSystem.FileExists(Path.Combine(str_USMTFolder, str_USMTSearchFile)) Then
                    Throw New Exception(Path.Combine(str_USMTFolder, str_USMTSearchFile) & " does not exist")
                End If
            Else
                str_USMTFolder = str_WMAFolder
                bln_IsUSMTInstalled = True
            End If
            sub_DebugMessage("Success. Found at: " & Path.Combine(str_USMTFolder, str_USMTSearchFile))
        Catch ex As Exception
            bln_IsUSMTInstalled = False
            sub_DebugMessage("WARNING: Unable to find USMT installation: " & ex.Message & "")
        End Try

        ' Present user with the option to download
        If Not bln_IsUSMTInstalled Then
            sub_DebugMessage("User Dialog...")
            Dim msgbox_Result As DialogResult = MsgBox(My.Resources.usmtNotFoundError, _
                            MsgBoxStyle.OkCancel + MsgBoxStyle.SystemModal + MsgBoxStyle.Critical, _
                            My.Resources.appTitle)
            Select Case msgbox_Result
                Case Windows.Forms.DialogResult.OK
                    Try
                        Dim webclient As New WebClient
                        Dim stopwatch_Download As New Stopwatch

                        AddHandler webclient.DownloadProgressChanged, AddressOf sub_WebClientDownloadProgress
                        AddHandler webclient.DownloadFileCompleted, AddressOf sub_WebClientDownloadComplete

                        ' Disable some form items while we check stuff
                        label_MigrationCurrentPhase.Text = "Checking for latest USMT version..."
                        button_Start.Enabled = False
                        ' Update the form
                        Application.DoEvents()

                        ' Check if the Manifest file already exists. If yes, delete it
                        If My.Computer.FileSystem.FileExists(str_bddManifestFileFullPath) Then
                            sub_DebugMessage(str_bddManifestFileFullPath & " already exists. Trying to delete...")
                            Try
                                My.Computer.FileSystem.DeleteFile(str_bddManifestFileFullPath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                sub_DebugMessage(str_bddManifestFileFullPath & " deleted successfully.")
                            Catch ex As Exception
                                Throw New Exception("ERROR: " & ex.Message & ". Skipping...")
                            End Try
                        End If

                        ' Check if the ComponentList file already exists. If yes, delete it
                        If My.Computer.FileSystem.FileExists(str_bddComponentFileFullPath) Then
                            sub_DebugMessage(str_bddComponentFileFullPath & " already exists. Trying to delete...")
                            Try
                                My.Computer.FileSystem.DeleteFile(str_bddComponentFileFullPath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                sub_DebugMessage(str_bddComponentFileFullPath & " deleted successfully.")
                            Catch ex As Exception
                                Throw New Exception("ERROR: " & ex.Message & ". Skipping...")
                            End Try
                        End If

                        ' Check for Network connectivity
                        sub_DebugMessage("Checking for network connection...")
                        If Not My.Computer.Network.IsAvailable Then
                            sub_DebugMessage("No internet connection available. Skipping...")
                            bln_SkipOnlineCheck = True
                        Else
                            Try
                                My.Computer.Network.Ping(My.Resources.appNetworkPingCheck)
                                sub_DebugMessage("Ping test to " & My.Resources.appNetworkPingCheck & " successful")
                            Catch ex As Exception
                                Throw New Exception("ERROR: Unable to ping " & My.Resources.appNetworkPingCheck & ". Skipping...")
                            End Try
                        End If

                        ' Download the new manifest file from the Microsoft site
                        sub_DebugMessage("Downloading Microsoft Deployment manifest file from " & str_bddManifestURL & "...")
                        Try
                            Dim uri_bddManifestDownloadURL As New Uri(str_bddManifestURL)

                            stopwatch_Download.Start()
                            bln_DownloadComplete = False
                            int_DownloadProgress = 0

                            webclient.DownloadFileAsync(uri_bddManifestDownloadURL, str_bddManifestFileFullPath)

                            Do Until bln_DownloadComplete
                                System.Threading.Thread.Sleep(1000)
                                Application.DoEvents()
                            Loop

                            stopwatch_Download.Stop()
                            sub_DebugMessage("Download successful (" & str_bddManifestFileFullPath & "). Took " & stopwatch_Download.Elapsed.Seconds & " second(s)")
                        Catch ex As Exception
                            Throw New Exception("ERROR: Download failed. " & ex.Message)
                        End Try

                        ' Extract the ComponentList from the manifest file
                        sub_DebugMessage("Extracting ComponentList file...")
                        Try
                            sub_ExpandCabinet(str_bddManifestFileFullPath, My.Resources.bddComponentFile, str_TempFolder)
                            sub_DebugMessage("Extraction successful (" & str_bddComponentFileFullPath & ")")
                        Catch ex As Exception
                            Throw New Exception("ERROR: Extraction failed: " & ex.Message)
                        End Try

                        ' Read XML file and check for relevant x86 / x64 USMT version
                        sub_DebugMessage("Checking USMT version against ComponentList...")
                        Try
                            Dim xml_Document As XmlDocument = New XmlDocument
                            xml_Document.Load(str_bddComponentFileFullPath)
                            Dim xml_Nodes As XmlNodeList = xml_Document.GetElementsByTagName("Component")
                            xml_Document = Nothing
                            For Each Xml_Node As XmlNode In xml_Nodes
                                ' Get GUID of each Component
                                xml_GUID = Xml_Node.Attributes.GetNamedItem("guid").Value
                                ' If there's a match to the current architecture, get the other details
                                If xml_GUID = str_USMTGUID Then
                                    xml_Architecture = Xml_Node.Attributes.GetNamedItem("Architecture").Value
                                    xml_InstallCondition = Xml_Node.Item("InstalledCondition").Attributes.GetNamedItem("RegistryKey").Value
                                    xml_DownloadURL = Xml_Node.Item("File").Attributes.GetNamedItem("URL").Value
                                    xml_DownloadFile = Xml_Node.Item("File").Attributes.GetNamedItem("FileName").Value
                                    xml_Description = Xml_Node.Item("ShortDescription").InnerText
                                    sub_DebugMessage("Match found: " & xml_Description & ": " & xml_GUID)
                                    Exit For
                                End If
                            Next
                            If xml_Description = Nothing Then
                                Throw New Exception("Unable to find USMT download information in the ComponentList")
                            End If
                        Catch ex As Exception
                            Throw New Exception("ERROR: Unable to parse the ComponentList correctly: " & ex.Message)
                        End Try

                        label_MigrationCurrentPhase.Text = "Downloading USMT Installation..."
                        Application.DoEvents()

                        sub_DebugMessage("Starting Download...")

                        Dim uri_USMTDownloadURL As New Uri(xml_DownloadURL)

                        stopwatch_Download.Start()
                        bln_DownloadComplete = False
                        int_DownloadProgress = 0

                        webclient.DownloadFileAsync(uri_USMTDownloadURL, Path.Combine(str_WMAFolder, xml_DownloadFile))

                        Do Until bln_DownloadComplete
                            System.Threading.Thread.Sleep(1000)
                            Application.DoEvents()
                        Loop

                        stopwatch_Download.Stop()
                        sub_DebugMessage("Download successful (" & Path.Combine(str_WMAFolder, xml_DownloadFile) & "). Took " & stopwatch_Download.Elapsed.Seconds & " second(s)")

                        label_MigrationCurrentPhase.Text = "Installing..."
                        Application.DoEvents()

                        sub_DebugMessage("Installing...")
                        Dim process As New Process
                        Dim processInfo As New ProcessStartInfo()
                        processInfo.FileName = Path.Combine(System.Environment.SystemDirectory, "MSIExec.Exe")
                        processInfo.Arguments = "/I """ & Path.Combine(str_WMAFolder, xml_DownloadFile) & """ /QB! Reboot=ReallySuppress"
                        processInfo.WorkingDirectory = str_WMAFolder
                        processInfo.UseShellExecute = False

                        ' Start the Installation
                        Try
                            process.StartInfo = processInfo
                            process.Start()
                            process.WaitForExit()
                        Catch ex As Exception
                            Throw New Exception(ex.Message)
                        End Try
                        sub_DebugMessage("Starting Installation...")

                    Catch ex As Exception
                        sub_DebugMessage("ERROR: Download / Installation failed: " & ex.Message, True)
                        appShutdown(24)
                    End Try
                Case Windows.Forms.DialogResult.Cancel
                    appShutdown(23)
            End Select
        End If

        label_MigrationCurrentPhase.Text = "..."
        button_Start.Enabled = True

    End Sub

    Private Function func_InitCheckForValidConfiguration()

        sub_DebugMessage()
        sub_DebugMessage("* Check for Valid WMA Configuration *")

        If My.Settings.MigrationNetworkLocation = str_WMAConfigNetworkCheck And Not My.Settings.MigrationNetworkLocationDisabled Then
            sub_DebugMessage("WARNING: Configuration does not seem to be valid")
            Return False
        Else
            sub_DebugMessage("Configuration seems to be valid")
            Return True
        End If

    End Function

    Private Sub sub_InitCheckForRuleSetFiles()

        sub_DebugMessage()
        sub_DebugMessage("* Check for RuleSet Files *")

        Dim bln_RuleSetMissing As Boolean = False
        Dim bln_RuleSetCopyFailed As Boolean = False

        ' *** Get Target OS and setup the rules for migration
        If bln_MigrationXPOnly Then
            ' Read the relevant configuration file
            sub_DebugMessage("Checking for XP Specific RuleSet files...")
            For Each ruleset As String In array_MigrationRuleSetXPOnly
                If Not My.Computer.FileSystem.FileExists(Path.Combine(str_WMAFolder, ruleset.Trim)) Then
                    sub_DebugMessage("WARNING: RuleSet file " & ruleset.Trim & " does not exist. Copying default file from USMT folder...")
                    Try
                        My.Computer.FileSystem.CopyFile(Path.Combine(str_USMTFolder, ruleset.Trim), Path.Combine(str_WMAFolder, ruleset.Trim), True)
                    Catch ex As Exception
                        sub_DebugMessage("ERROR: Unable to copy file - " & Path.Combine(str_USMTFolder, ruleset.Trim))
                        bln_RuleSetCopyFailed = True
                    End Try
                    bln_RuleSetMissing = True
                End If
            Next

        Else
            For Each ruleset As String In array_MigrationRuleSet
                If Not My.Computer.FileSystem.FileExists(Path.Combine(str_WMAFolder, ruleset.Trim)) Then
                    sub_DebugMessage("WARNING: RuleSet file " & ruleset.Trim & " does not exist. Copying default file from USMT folder...")
                    Try
                        My.Computer.FileSystem.CopyFile(Path.Combine(str_USMTFolder, ruleset.Trim), Path.Combine(str_WMAFolder, ruleset.Trim), True)
                    Catch ex As Exception
                        sub_DebugMessage("ERROR: Unable to copy file - " & Path.Combine(str_USMTFolder, ruleset.Trim))
                        bln_RuleSetCopyFailed = True
                    End Try
                    bln_RuleSetMissing = True
                End If
            Next
        End If

        If bln_RuleSetMissing Then
            If bln_RuleSetCopyFailed Then
                sub_DebugMessage("ERROR: RuleSet files were not found in the WMA folder. An attempt to copy the default RuleSets from USMT failed. The application will now close.", True)
                appShutdown(21)
            End If
            sub_DebugMessage("INFO: RuleSet files were not found in the WMA folder. This may be your first time running WMA, so the default RuleSet files have been copied from USMT", True)
        Else
            sub_DebugMessage("All RuleSet files exist")
        End If

    End Sub

    Private Sub sub_InitParseCommandLine()

        sub_DebugMessage()
        sub_DebugMessage("* Parse Command Line *")

        Try
            Dim col_CommandLineArguments As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Application.CommandLineArgs
            Dim int_Count As Integer
            If Not col_CommandLineArguments.Count > 0 Then
                Exit Sub
            End If
            For int_Count = 0 To col_CommandLineArguments.Count - 1
                sub_DebugMessage("Checking: " & col_CommandLineArguments(int_Count).ToUpper)
                Select Case col_CommandLineArguments(int_Count).ToUpper
                    Case "/PROGRESSONLY"
                        sub_DebugMessage("Match: Progress Only Mode")
                        bln_AppProgressOnlyMode = True
                        If (int_Count + 1) < col_CommandLineArguments.Count Then
                            Select Case col_CommandLineArguments(int_Count + 1).ToUpper
                                Case "CAPTURE"
                                    sub_DebugMessage("Type: Capture")
                                    tabcontrol_MigrationType.SelectedTab = tabpage_Capture
                                Case "RESTORE"
                                    sub_DebugMessage("Type: Restore")
                                    tabcontrol_MigrationType.SelectedTab = tabpage_Restore
                                Case Else
                                    Throw New Exception("/PROGRESSONLY was specified without CAPTURE or RESTORE")
                            End Select
                        Else
                            Throw New Exception("/PROGRESSONLY was specified without CAPTURE or RESTORE")
                        End If
                    Case "/MIGOVERWRITEEXISTING"
                        sub_DebugMessage("Match: Overwrite Existing Migration Folders")
                        bln_MigrationOverwriteExistingFolders = True
                    Case "/MIGMAXOVERRIDE"
                        sub_DebugMessage("Match: Override Maximum Migration Size Limit")
                        bln_migrationMaxOverride = True
                    Case "/MIGFOLDER"
                        sub_DebugMessage("Match: Alternate Migration Folder")
                        If (int_Count + 1) < col_CommandLineArguments.Count Then
                            str_MigrationLocationOther = col_CommandLineArguments(int_Count + 1).ToUpper
                            bln_MigrationLocationUseOther = True
                            sub_DebugMessage("Folder: " & str_MigrationFolder)
                        Else
                            Throw New Exception("/MIGFOLDER was specified without a Migration Folder")
                        End If
                    Case "/XPONLY"
                        sub_DebugMessage("Match: XP Only Mode")
                        ' Do not enable XPOnly mode if the current OS is Vista / Win 7
                        If dbl_OSVersion < 6.0 Then
                            bln_MigrationXPOnly = True
                        Else
                            sub_DebugMessage("WARNING: XP Only Mode is specified but OS is not XP! Disabling...")
                        End If
                    Case "/CHANGEDOMAIN"
                        sub_DebugMessage("Match: Change Domain Mode")
                        If (int_Count + 1) < col_CommandLineArguments.Count Then
                            str_MigrationDomainChange = col_CommandLineArguments(int_Count + 1).ToUpper
                            sub_DebugMessage("New Domain Name: " & str_MigrationDomainChange)
                        Else
                            Throw New Exception("/CHANGEDOMAIN was specified without a New Domain Name")
                        End If
                    Case "/PRIMARYDATADRIVE"
                        sub_DebugMessage("Match: Change Primary Data Drive Mode")
                        If (int_Count + 1) < col_CommandLineArguments.Count Then
                            str_PrimaryDataDrive = col_CommandLineArguments(int_Count + 1).ToUpper
                            sub_DebugMessage("New Primary Data Drive: " & str_PrimaryDataDrive)
                        Else
                            Throw New Exception("/PRIMARYDATADRIVE was specified without a Primary Data Drive")
                        End If
                    Case "/MULTIUSER"
                        sub_DebugMessage("Match: Multi-User Mode")
                        ' Enable Multi-User Mode
                        radiobox_WorkstationDetails2.Checked = True
                End Select
            Next
        Catch ex As Exception
            sub_DebugMessage("ERROR: A problem occurred while processing command-line parameters: " & vbNewLine & ex.Message, True)
            appShutdown(My.Resources.exitCodeCMDLineParamError)
        End Try

    End Sub

    Private Sub sub_ExpandCabinet(ByVal str_CABFileName As String, ByVal str_ExtractFileName As String, ByVal str_DestinationPath As String)

        sub_DebugMessage("Expanding Cabinet...")

        Dim process = New Process
        Dim processInfo As New ProcessStartInfo

        ' Setup and Start the Expand process
        Try

            processInfo.CreateNoWindow = True
            processInfo.UseShellExecute = False
            processInfo.FileName = Path.Combine(Path.Combine(Environment.GetEnvironmentVariable("windir"), "system32"), "expand.exe")
            processInfo.Arguments = """" & str_CABFileName & """ -F:" & str_ExtractFileName & " """ & str_DestinationPath & """"

            sub_DebugMessage("Process: " & processInfo.FileName & " " & processInfo.Arguments)

            process.StartInfo = processInfo
            process.Start()

            ' Wait until the process exits and check the error code
            process.WaitForExit()

            ' Get the exit code
            If Not process.ExitCode = 0 Then
                Throw New Exception("Process returned error code: " & process.ExitCode)
            End If

        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Shared Sub sub_WebClientDownloadProgress(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs)
        ' Update progress as necessary
        If e.ProgressPercentage > int_DownloadProgress Then
            ' Update counter
            int_DownloadProgress = e.ProgressPercentage
            ' Write to log file
            sub_DebugMessage("Downloading file: " & CStr(CInt(e.BytesReceived / 1024)) + " of " + CStr(CInt(e.TotalBytesToReceive / 1024)) + " KB, " + CStr(e.ProgressPercentage) & "% Complete...")
            ' Update Display
            form_Migration.label_MigrationCurrentPhase.Text = "Downloading file: " & CStr(CInt(e.BytesReceived / 1024)) + " of " + CStr(CInt(e.TotalBytesToReceive / 1024)) + " KB, " + CStr(e.ProgressPercentage) & "% Complete..."
            form_Migration.label_MigrationCurrentPhase.Refresh()
        End If
    End Sub

    Private Shared Sub sub_WebClientDownloadComplete(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
        bln_DownloadComplete = True
    End Sub

#End Region

#Region "USB Support"

    ' Initialise USB Support
    Private Sub sub_USBInitialise()

        sub_DebugMessage()
        sub_DebugMessage("* USB Initialisation *")

        ' *** Check if a USB disk drive is connected
        Try
            sub_DebugMessage("Checking if USB drive is connected...")
            Dim wmiQuery As String = "SELECT * FROM Win32_DiskDrive"
            Dim searcher As New ManagementObjectSearcher(wmiQuery)
            For Each obj_Query As ManagementObject In searcher.Get()
                ' If this is a USB disk, and the size exceeds what is specified in the settings file
                If obj_Query("InterfaceType") = "USB" And (obj_Query("Size") / 1048576) >= int_MigrationMinUSBDiskSize Then
                    sub_DebugMessage("Drive Found: " & func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"))
                    ' Check SMART tolerences
                    sub_DebugMessage("Drive SMART Status: " & obj_Query("Status"))
                    Select Case obj_Query("Status")
                        Case "OK"
                            ' Automatically use the USB drive if the setting is present
                            If bln_MigrationUSBAutoUseIfAvailable Then
                                sub_DebugMessage("Auto-Use of USB drive: " & func_USBGetDriveLetter(obj_Query("Name")))
                                bln_MigrationLocationUseUSB = True
                                str_MigrationLocationUSB = Path.GetFullPath(func_USBGetDriveLetter(obj_Query("Name")))
                            Else

                                ' Present option to use new drive
                                sub_DebugMessage("User Dialog...")
                                Dim msgbox_Result As DialogResult = MsgBox(My.Resources.usbDeviceDescription & " " & _
                                My.Resources.usbDeviceConnectedStartup & " " & My.Resources.usbDeviceUseDrive, _
                                                MsgBoxStyle.YesNo + MsgBoxStyle.SystemModal + MsgBoxStyle.Question, _
                                                func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"))
                                Select Case msgbox_Result
                                    ' Set useUSBMigrationLocation to true and set new USB drive location
                                    Case Windows.Forms.DialogResult.Yes
                                        bln_MigrationLocationUseUSB = True
                                        str_MigrationLocationUSB = Path.GetFullPath(func_USBGetDriveLetter(obj_Query("Name")))
                                        sub_DebugMessage("User chose to use drive: " & func_USBGetDriveLetter(obj_Query("Name")))
                                        Exit Sub
                                    Case Else
                                        sub_DebugMessage("User chose not to use drive: " & func_USBGetDriveLetter(obj_Query("Name")))
                                End Select
                            End If
                        Case Else
                            ' Display error about drive
                            sub_DebugMessage("ERROR: " & My.Resources.usbDeviceDescription & " " & _
                                    My.Resources.usbDeviceConnectedStartup & " " & My.Resources.usbDeviceSMARTFail1 & _
                                    vbNewLine & vbNewLine & My.Resources.usbDeviceSMARTFail2 & vbNewLine & _
                                    func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"), True)
                    End Select
                End If
            Next

        Catch ex As Exception
            sub_DebugMessage("ERROR: Failed to determine if USB drive was connected. " & ex.Message)
        End Try

        ' *** Start watching USB device State Change Events
        Dim wmiEventQuery As String = "SELECT * FROM __InstanceOperationEvent WITHIN 10 WHERE TargetInstance ISA ""Win32_DiskDrive"""
        eventwatcher_USBStateChange = New ManagementEventWatcher(wmiEventQuery)
        sub_DebugMessage("Starting USB event watcher...")
        Try
            eventwatcher_USBStateChange.Start()
            sub_DebugMessage("USB event watcher started")
        Catch ex As Exception
            sub_DebugMessage("ERROR: Failed to start USB event watcher. " & ex.Message)
        End Try

    End Sub

    ' Monitor USB State Change Events
    Private Sub sub_USBStateChangeEvent(ByVal sender As Object, ByVal e As System.Management.EventArrivedEventArgs) Handles eventwatcher_USBStateChange.EventArrived

        sub_DebugMessage()
        sub_DebugMessage("* USB State Change Event *")

        Try
            Dim obj_Base, obj_Query As ManagementBaseObject
            obj_Base = CType(e.NewEvent, ManagementBaseObject)
            obj_Query = CType(obj_Base("TargetInstance"), ManagementBaseObject)

            sub_DebugMessage("Checking if USB drive has been connected / disconnected...")
            Select Case obj_Base.ClassPath.ClassName
                ' If creation event...
                Case "__InstanceCreationEvent"
                    ' If this is a USB disk, and the size exceeds what is specified in the settings file
                    If obj_Query("InterfaceType") = "USB" And (obj_Query("Size") / 1048576) >= int_MigrationMinUSBDiskSize Then
                        sub_DebugMessage("Drive Found: " & func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"))
                        ' Check SMART tolerences
                        sub_DebugMessage("Drive SMART Status: " & obj_Query("Status"))
                        Select Case obj_Query("Status")
                            Case "OK"
                                ' Check if USB migration is already selected (ie, prior USB device selected)
                                Select Case bln_MigrationLocationUseUSB
                                    Case True
                                        ' Present option to use new drive instead
                                        sub_DebugMessage("User Dialog...")
                                        Dim msgbox_Result As DialogResult = MsgBox(My.Resources.usbDeviceDescriptionAdditional & " " & _
                                                        My.Resources.usbDeviceConnectedEvent & " " & My.Resources.usbDeviceUseDrive, _
                                                        MsgBoxStyle.YesNo + MsgBoxStyle.SystemModal + MsgBoxStyle.Question, _
                                                        func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"))
                                        Select Case msgbox_Result
                                            ' Set new USB drive location
                                            Case Windows.Forms.DialogResult.Yes
                                                str_MigrationLocationUSB = Path.GetFullPath(func_USBGetDriveLetter(obj_Query("Name")))
                                                sub_DebugMessage("User chose to use drive: " & func_USBGetDriveLetter(obj_Query("Name")))
                                                Exit Sub
                                            Case Else
                                                sub_DebugMessage("User chose not to use drive: " & func_USBGetDriveLetter(obj_Query("Name")))
                                        End Select
                                        ' Present option to use drive
                                    Case False
                                        Dim msgbox_Result As DialogResult = MsgBox(My.Resources.usbDeviceDescription & " " & _
                                                        My.Resources.usbDeviceConnectedEvent & " " & My.Resources.usbDeviceUseDrive, _
                                                        MsgBoxStyle.YesNo + MsgBoxStyle.SystemModal + MsgBoxStyle.Question, _
                                                        func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"))
                                        Select Case msgbox_Result
                                            ' Set useUSBMigrationLocation to true and set new USB drive location
                                            Case Windows.Forms.DialogResult.Yes
                                                bln_MigrationLocationUseUSB = True
                                                str_MigrationLocationUSB = Path.GetFullPath(func_USBGetDriveLetter(obj_Query("Name")))
                                                Exit Sub
                                        End Select
                                End Select
                            Case Else
                                ' Display error about drive
                                sub_DebugMessage("ERROR: " & My.Resources.usbDeviceDescription & " " & _
                                        My.Resources.usbDeviceConnectedEvent & " " & My.Resources.usbDeviceSMARTFail1 & _
                                        vbNewLine & vbNewLine & My.Resources.usbDeviceSMARTFail2 & vbNewLine & _
                                        func_USBGetDriveLetter(obj_Query("Name")) & " - " & obj_Query("Caption"), True)
                        End Select

                    End If
                    ' If deletion event...
                Case "__InstanceDeletionEvent"
                    If bln_MigrationLocationUseUSB = True Then
                        sub_DebugMessage("USB drive has been removed. USB mode disabled")
                        ' Set usbUSBDrive to false and display message
                        bln_MigrationLocationUseUSB = False
                        sub_DebugMessage("WARNING: " & My.Resources.usbDeviceDescriptionCurrent & " " & _
                                My.Resources.usbDeviceDisconnectedEvent & " " & _
                                vbNewLine & vbNewLine & My.Resources.usbDeviceSwitchToStandard & vbNewLine & _
                                obj_Query("Caption"), True)
                    End If
            End Select

        Catch ex As Exception
            sub_DebugMessage("ERROR: Failed to determine if USB drive was connected / disconnected. " & ex.Message)
        End Try

    End Sub

    ' Use WMI to get the Drive letter based on the passed in WMI Name
    Private Function func_USBGetDriveLetter(ByVal Name As String) As String

        Dim objquery_Partition, objquery_Disk As ObjectQuery
        Dim searcher_Partition, searcher_Disk As ManagementObjectSearcher
        Dim obj_Partition, obj_Disk As ManagementObject
        Dim str_Return As String = ""

        ' WMI queries use the "\" as an escape charcter
        Name = Replace(Name, "\", "\\")

        ' First we map the Win32_DiskDrive instance with the association called
        ' Win32_DiskDriveToDiskPartition.  Then we map the Win23_DiskPartion
        ' instance with the assocation called Win32_LogicalDiskToPartition

        Try

            objquery_Partition = New ObjectQuery("ASSOCIATORS OF {Win32_DiskDrive.DeviceID=""" & Name & """} WHERE AssocClass = Win32_DiskDriveToDiskPartition")
            searcher_Partition = New ManagementObjectSearcher(objquery_Partition)
            For Each obj_Partition In searcher_Partition.Get()

                objquery_Disk = New ObjectQuery("ASSOCIATORS OF {Win32_DiskPartition.DeviceID=""" & obj_Partition("DeviceID") & """} WHERE AssocClass = Win32_LogicalDiskToPartition")
                searcher_Disk = New ManagementObjectSearcher(objquery_Disk)
                For Each obj_Disk In searcher_Disk.Get()
                    str_Return &= obj_Disk("Name") & ","
                Next
            Next

        Catch ex As Exception
            sub_DebugMessage("ERROR: Failed to determine USB drive letter. " & ex.Message)
            Return Nothing
        End Try

        Return str_Return.Trim(","c)

    End Function

#End Region

#Region "Health Check"

    Private Sub sub_HealthCheckStart()

        sub_DebugMessage()
        sub_DebugMessage("* Start Health Check *")

        ' Initialise the Health Check Class
        class_HealthCheck = New classHealthCheck

        ' Reset Status items
        Dim str_PreviousStatusMessage As String = Nothing
        Dim str_StatusMessage As String = Nothing
        progressbar_Migration.Value = 0

        ' Set the Health Check to being in progress
        bln_HealthCheckInProgress = True

        ' Start the Health Check
        Try
            sub_DebugMessage("Spinning up Health Check class...")
            class_HealthCheck.Spinup()

        Catch ex As Exception
            class_HealthCheck.SpinDown()
            sub_DebugMessage("ERROR: " & ex.Message, True)
            label_MigrationCurrentPhase.Text = ex.Message
            Exit Sub
        End Try

    End Sub

    Private Sub sub_HealthCheckFinish() Handles class_HealthCheck.HealthCheckFinished

        ' Handle thread invoking so that the UI can be updated cross-thread
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf sub_HealthCheckFinish))
        Else

            sub_DebugMessage()
            sub_DebugMessage("* Finish Health Check *")

            Try

                ' Max out the progress bar
                progressbar_Migration.Value = 100

                sub_DebugMessage("Spinning down Health Check Class...")
                class_HealthCheck.SpinDown()

                ' Check the exit code. Let's not take chances, if it's not 0, then it's an error.
                Select Case class_HealthCheck.ExitCode
                    Case 0
                        sub_DebugMessage("Success, no errors found")
                        label_MigrationCurrentPhase.Text = "Health Check: " & My.Resources.diskScanResultOk
                        bln_HealthCheckStatusOk = True
                    Case 1
                        sub_DebugMessage("Success, CHKDSK has detected and fixed major errors")
                        label_MigrationCurrentPhase.Text = "Health Check: " & My.Resources.diskScanResultOk
                        bln_HealthCheckStatusOk = True
                    Case 2
                        sub_DebugMessage("Success, CHKDSK has detected and fixed minor inconsistencies")
                        label_MigrationCurrentPhase.Text = "Health Check: " & My.Resources.diskScanResultOk
                        bln_HealthCheckStatusOk = True
                    Case 999
                        sub_DebugMessage("Health Check Cancelled")
                        bln_MigrationCancelled = True
                    Case Else
                        sub_DebugMessage("ERROR: Health Check Failed")
                        label_MigrationCurrentPhase.Text = "Health Check: " & My.Resources.diskScanResultErrors

                        ' If in Progress Only Mode, exit the app
                        If bln_AppProgressOnlyMode Then
                            appShutdown(5)
                        End If

                        ' See if the user wants to fix the problem or continue
                        sub_DebugMessage("Prompting User for action...")
                        Dim msgbox_Result As DialogResult = MsgBox(My.Resources.diskScanFixMessage, MsgBoxStyle.Exclamation + MsgBoxStyle.YesNoCancel, My.Resources.appTitle)
                        ' If yes, start CHKDSK
                        If msgbox_Result = MsgBoxResult.Yes Then
                            sub_DebugMessage("INFO: User chose to fix disk and reboot")
                            System.Diagnostics.Process.Start("CMD", "/C Echo Y | CHKDSK /F /R")
                            System.Diagnostics.Process.Start("Shutdown", "-R -T 10 -C ""CHKDSK Initialised by " & My.Resources.appTitle & ". Restarting Workstation""")
                            appShutdown(1)
                            ' If no, continue with the migration
                        ElseIf msgbox_Result = MsgBoxResult.No Then
                            sub_DebugMessage("WARNING: " & My.Resources.diskScanFixCancelledMessage, True)
                            ' If cancel, close the application
                        Else
                            sub_DebugMessage("INFO: User chose to exit application")
                            appShutdown(8)
                        End If
                End Select

            Catch ex As Exception
                bln_MigrationStatusOk = False
                label_MigrationCurrentPhase.Text = ex.Message
                sub_DebugMessage(ex.Message, True)
            End Try

            bln_HealthCheckInProgress = False

            ' Start Migration
            If Not bln_MigrationCancelled Then
                sub_MigrationSetup()
            End If

            If Not bln_MigrationCancelled Then
                sub_MigrationStart()
            End If

            If bln_MigrationCancelled Then
                sub_SupportFormControlsReset()
            End If

        End If

    End Sub

    Private Sub sub_HealthCheckProgressMonitor() Handles class_HealthCheck.ProgressUpdate

        ' Handle thread invoking so that the UI can be updated cross-thread
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf sub_HealthCheckProgressMonitor))
        Else

            ' Get the current status
            If Not class_HealthCheck.Progress = Nothing Then
                If Not class_HealthCheck.PercentComplete = Nothing Or class_HealthCheck.PercentComplete = Nothing = 0 Then
                    str_StatusMessage = class_HealthCheck.Progress & " (" & class_HealthCheck.PercentComplete & My.Resources.diskScanPercent & ")"
                Else
                    str_StatusMessage = class_HealthCheck.Progress
                End If
            End If

            ' If the status has changed, output
            If Not str_StatusMessage = str_PreviousStatusMessage Then
                sub_DebugMessage(str_StatusMessage)
                label_MigrationCurrentPhase.Text = str_StatusMessage
                progressbar_Migration.Value = class_HealthCheck.PercentComplete
            End If

            str_PreviousStatusMessage = str_StatusMessage

        End If

    End Sub

#End Region

#Region "Migration"

    Private Sub sub_MigrationInitialise()

        sub_DebugMessage()
        sub_DebugMessage("* Migration Initialise *")

        bln_MigrationCancelled = False

        ' Initialise the Migration Class
        class_Migration = New classMigration

        ' Disable / Update form controls
        button_AdvancedSettings.Enabled = False
        tabcontrol_MigrationType.Enabled = False
        button_Start.Text = "&Stop"

        ' Enable / Update status items
        group_Status.Enabled = True
        progressbar_Migration.Value = 0
        progressbar_Migration.Visible = True
        label_MigrationCurrentPhase.ForeColor = Color.Black
        label_MigrationCurrentPhase.Text = "Initialising..."

        ' Start the Health Check
        If bln_HealthCheck Then
            ' Skip if LoadState - Only needed for capture, not restore.
            If Not str_MigrationType = "LOADSTATE" Then
                sub_HealthCheckStart()
            Else
                ' Start Migration
                If Not bln_MigrationCancelled Then
                    sub_MigrationSetup()
                End If

                If Not bln_MigrationCancelled Then
                    sub_MigrationStart()
                End If
            End If
        Else
            ' Start Migration
            If Not bln_MigrationCancelled Then
                sub_MigrationSetup()
            End If

            If Not bln_MigrationCancelled Then
                sub_MigrationStart()
            End If
        End If

    End Sub

    Private Sub sub_MigrationSetup()

        sub_DebugMessage()
        sub_DebugMessage("* Migration Setup *")

        ' Reset all migration settings in the array
        arraylist_MigrationArguments.Clear()

        ' *** Get Target OS and setup the rules for migration
        If bln_MigrationXPOnly Then
            ' Read the relevant configuration file
            For Each ruleset As String In array_MigrationRuleSetXPOnly
                arraylist_MigrationArguments.Add("/I:""" & Path.Combine(str_WMAFolder, ruleset.Trim) & """")
            Next
            If My.Computer.FileSystem.FileExists(Path.Combine(str_WMAFolder, str_MigrationConfigFileXP)) Then
                sub_DebugMessage("Config file exists and will be used: " & Path.Combine(str_WMAFolder, str_MigrationConfigFileXP))
                arraylist_MigrationArguments.Add("/Config:""" & Path.Combine(str_WMAFolder, str_MigrationConfigFileXP) & """")
            Else
                sub_DebugMessage("Config file does not exist. Standard migration will be performed")
            End If

        Else
            For Each ruleset As String In array_MigrationRuleSet
                arraylist_MigrationArguments.Add("/I:""" & Path.Combine(str_WMAFolder, ruleset.Trim) & """")
            Next
            If My.Computer.FileSystem.FileExists(Path.Combine(str_WMAFolder, str_MigrationConfigFile)) Then
                sub_DebugMessage("Config file exists and will be used: " & Path.Combine(str_WMAFolder, str_MigrationConfigFile))
                arraylist_MigrationArguments.Add("/Config:""" & Path.Combine(str_WMAFolder, str_MigrationConfigFile) & """")
            Else
                sub_DebugMessage("Config file does not exist. Standard migration will be performed")
            End If

        End If

        sub_DebugMessage("Migration Type: " & str_MigrationType)
        Select Case str_MigrationType
            Case "SCANSTATE"

                ' *** Check where the Backup is being performed to...
                If bln_MigrationLocationUseOther Then
                    sub_DebugMessage("Migrating from Alternate location: " & str_MigrationLocationOther)
                    str_MigrationFolder = str_MigrationLocationOther
                ElseIf bln_MigrationLocationUseUSB Then
                    sub_DebugMessage("Migrating from USB drive: " & str_MigrationLocationUSB)
                    str_MigrationFolder = str_MigrationLocationUSB
                ElseIf bln_MigrationLocationNetworkDisabled Then
                    sub_DebugMessage("ERROR: Network-based migrations have been disabled by your IT Administrator. A USB drive is required to proceed with the migration.", True)
                    bln_MigrationCancelled = True
                    Exit Sub
                Else
                    sub_DebugMessage("Migrating from Network Location: " & str_MigrationLocationNetwork)
                    ' If this is a network migration, verify that pass is accessible
                    If str_MigrationLocationNetwork.EndsWith(Path.VolumeSeparatorChar) Then
                        str_MigrationLocationNetwork = str_MigrationLocationNetwork & "\"
                    End If
                    str_MigrationFolder = str_MigrationLocationNetwork
                    If Not My.Computer.FileSystem.DirectoryExists(str_MigrationFolder) Then
                        sub_DebugMessage("ERROR: The Network-based migration location is not available. Please verify you have network connectivity. If the problem persists, contact your IT Administrator.", True)
                        bln_MigrationCancelled = True
                        Exit Sub
                    End If
                End If

                ' *** Generate the folder name for the migration...
                ' *** And verify it doesn't already exist...
                If My.Computer.FileSystem.DirectoryExists(Path.Combine(str_MigrationFolder, str_EnvComputerName & "_" & str_EnvUserName)) Then
                    Try
                        If Not bln_MigrationOverwriteExistingFolders Then
                            ' Present option to remove existing migration information
                            Dim msgbox_Result As DialogResult = MsgBox(My.Resources.migrationOverwriteExistingFolder, _
                                            MsgBoxStyle.YesNo + MsgBoxStyle.SystemModal + MsgBoxStyle.Question, _
                                            str_EnvComputerName & "_" & str_EnvUserName)
                            Select Case msgbox_Result
                                Case Windows.Forms.DialogResult.Yes
                                    My.Computer.FileSystem.DeleteDirectory(Path.Combine(str_MigrationFolder, str_EnvComputerName & "_" & str_EnvUserName), FileIO.DeleteDirectoryOption.DeleteAllContents)
                            End Select
                        Else
                            My.Computer.FileSystem.DeleteDirectory(Path.Combine(str_MigrationFolder, str_EnvComputerName & "_" & str_EnvUserName), FileIO.DeleteDirectoryOption.DeleteAllContents)
                        End If
                    Catch exPrivilege As System.Security.AccessControl.PrivilegeNotHeldException
                        sub_DebugMessage("ERROR: " & My.Resources.migrationDeleteExistingError & vbNewLine & vbNewLine & _
                                exPrivilege.Message, True)
                    Catch ex As Exception
                        sub_DebugMessage("ERROR: " & My.Resources.migrationDeleteExistingError & vbNewLine & vbNewLine & _
                                ex.Message, True)
                    End Try
                End If

                str_MigrationFolder = Path.Combine(str_MigrationFolder, str_EnvComputerName & "_" & str_EnvUserName)

                If bln_MigrationXPOnly Then
                    ' Read the relevant configuration file
                    arraylist_MigrationArguments.Add("/TargetXP")
                End If

                ' Create the folder structure
                sub_MigrationCreateFolderStructure(Path.Combine(str_MigrationFolder, str_MigrationDataStoreFolder))
                sub_MigrationCreateFolderStructure(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder))

                ' Add the standard arguments to Argument List
                arraylist_MigrationArguments.Add("/LocalOnly")

            Case "LOADSTATE"

                ' If migrating all accounts
                If bln_MigrationSettingsAllUsers Then

                    If str_MigrationRestoreAccountsPassword = "" Then
                        ' Create local accounts with blank password
                        arraylist_MigrationArguments.Add("/LAC")
                    Else
                        ' Create local accounts with pre-specified password
                        arraylist_MigrationArguments.Add("/LAC:""" & str_MigrationRestoreAccountsPassword & """")
                    End If

                    If bln_MigrationRestoreAccountsEnabled Then
                        ' Set new locally created accounts to Enabled
                        arraylist_MigrationArguments.Add("/LAE")
                    End If

                End If

                ' If a Domain Change is specified
                If Not str_MigrationDomainChange = "" Then
                    arraylist_MigrationArguments.Add("/MD:" & str_EnvDomain & ":" & str_MigrationDomainChange)
                End If

        End Select

        ' *** Set the migration location to the dataStore
        arraylist_MigrationArguments.Add("""" & Path.Combine(str_MigrationFolder, str_MigrationDataStoreFolder) & """")

        ' ***  Test Mode (No Compression)
        If bln_MigrationCompressionDisabled Then
            arraylist_MigrationArguments.Add("/NoCompress")
        End If

        ' *** If not migrating all accounts...
        If Not bln_MigrationSettingsAllUsers Then
            ' Include current user (either domain or local) and exclude all others
            arraylist_MigrationArguments.Add("/UI:""" & My.User.CurrentPrincipal.Identity.Name & """")
            arraylist_MigrationArguments.Add("/UE:*\*")
            ' *** Otherwise...
        Else
            ' Exclude accounts older than the specified number of days
            If int_MigrationExclusionsOlderThanDays > 0 Then
                arraylist_MigrationArguments.Add("/UEL:" & int_MigrationExclusionsOlderThanDays & "")
            End If
            ' Exclude domain accounts specified in settings file
            For Each exclusion As String In array_MigrationExclusionsDomain
                arraylist_MigrationArguments.Add("/UE:""" & str_EnvDomain & "\" & exclusion.Trim & """")
            Next
            ' If not migrating local accounts...
            If Not bln_MigrationSettingsLocalAccounts Then
                ' Exclude local accounts
                arraylist_MigrationArguments.Add("/UE:" & str_EnvComputerName & "\*")
                ' *** Otherwise...
            Else
                ' Exclude local accounts specified in settings file
                For Each exclusion As String In array_MigrationExclusionsLocal
                    arraylist_MigrationArguments.Add("/UE:""" & str_EnvComputerName & "\" & exclusion.Trim & """")
                Next
            End If
        End If

        ' *** Get Encryption Settings
        If Not bln_MigrationEncryptionDisabled Then
            If Not bln_MigrationCompressionDisabled Then
                Select Case str_MigrationType
                    Case "SCANSTATE"
                        arraylist_MigrationArguments.Add("/Encrypt")
                    Case "LOADSTATE"
                        arraylist_MigrationArguments.Add("/Decrypt")
                End Select
                If Not bln_MigrationEncryptionCustom Then
                    ' Set scanstate arguments to use standard encryption key
                    arraylist_MigrationArguments.Add("/Key:""" & str_MigrationEncryptionDefaultKey & """")
                Else
                    formCustomEncryption.tbxCustomEncryptionKey1.Text = str_customEncryptionKey
                    ' Set scanstate arguments to use custom encryption key (built from standard key, and user specified)
                    arraylist_MigrationArguments.Add("/Key:""" & str_MigrationEncryptionDefaultKey & str_customEncryptionKey & """")
                End If
            End If
        End If

        arraylist_MigrationArguments.Add("/V:" & int_MigrationUSMTLoggingType)

    End Sub

    Private Sub sub_MigrationStart()

        sub_DebugMessage()
        sub_DebugMessage("* Start Migration *")

        str_PreviousStatusMessage = Nothing
        str_StatusMessage = Nothing
        dtm_StartTime = Now
        ' Set the migration type
        class_Migration.Type = str_MigrationType

        ' Add the standard arguments to Argument List
        arraylist_MigrationArguments.Add("/R:5")
        arraylist_MigrationArguments.Add("/W:3")
        arraylist_MigrationArguments.Add("/C")

        ' Transfer the Arguments list to the Migration Class
        class_Migration.Arguments = arraylist_MigrationArguments


        ' *** Run Pre-Migration Scripts
        sub_DebugMessage("Running Pre-Migration Scripts...")
        If Not func_MigrationSupportScripts("Pre") Then
            label_MigrationCurrentPhase.ForeColor = Color.Red
            label_MigrationCurrentPhase.Text = My.Resources.migrationPreMigrationScriptFail
            If bln_AppProgressOnlyMode Then
                appShutdown(My.Resources.exitCodePreMigrationScriptFail)
            End If
            Exit Sub
        End If

        ' Make sure the Data Size Checks only run once
        bln_SizeChecksDone = False

        bln_MigrationInProgress = True

        Try
            sub_DebugMessage("Spinning up Migration class...")
            class_Migration.Spinup()

        Catch ex As Exception
            class_Migration.SpinDown()
            sub_DebugMessage(ex.Message, True)
            label_MigrationCurrentPhase.Text = ex.Message
            Exit Sub
        End Try

    End Sub

    Private Sub sub_MigrationFinish() Handles class_Migration.MigrationFinished

        ' Handle thread invoking so that the UI can be updated cross-thread
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf sub_MigrationFinish))
        Else

            sub_DebugMessage()
            sub_DebugMessage("* Finish Migration *")

            Try

                sub_DebugMessage("Spinning down Migration Class...")
                class_Migration.SpinDown()

                sub_DebugMessage("Checking Exit Code: " & class_Migration.ExitCode)
                ' Check the exit code. 
                Select Case class_Migration.ExitCode
                    Case 0, 1073741819, "-1073741819"
                        bln_MigrationStatusOk = True
                        ' Check if an abort has occurred and end the migration
                    Case 12
                        Throw New Exception("You must be an administrator to migrate one or more of the files or settings that are in the store. Log on as an administrator and try again.")
                    Case 999
                        bln_MigrationCancelled = True
                        Throw New Exception("Migration Cancelled")
                    Case Else
                        Throw New Exception("An unknown error occurred. Check the USMT log files for more details")
                End Select

            Catch ex As Exception
                bln_MigrationStatusOk = False
                label_MigrationCurrentPhase.Text = ex.Message
                sub_DebugMessage(ex.Message, True)
            End Try

            ' *** Run Post-Migration Scripts
            If bln_MigrationStatusOk Then
                sub_DebugMessage("Running Post-Migration Scripts...")
                If Not func_MigrationSupportScripts("Post") Then
                    label_MigrationCurrentPhase.ForeColor = Color.Red
                    label_MigrationCurrentPhase.Text = My.Resources.migrationPostMigrationScriptFail
                    If bln_AppProgressOnlyMode Then
                        appShutdown(My.Resources.exitCodePostMigrationScriptFail)
                    End If
                    Exit Sub
                End If
            End If

            ' Write WMA XML Log File
            Try
                sub_DebugMessage("Building XML Log File...")
                Dim xml_LogFile As XmlTextWriter = New XmlTextWriter(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder) & "\WMA_" & str_MigrationType & ".XML", Nothing)
                xml_LogFile.WriteStartDocument()
                xml_LogFile.WriteComment(My.Resources.appTitle & " " & My.Resources.appBuild)
                xml_LogFile.WriteStartElement("MigAssistant")
                xml_LogFile.WriteStartElement("Computer")
                xml_LogFile.WriteAttributeString("ComputerName", str_EnvComputerName)
                xml_LogFile.WriteAttributeString("User", str_EnvUserName)
                xml_LogFile.WriteAttributeString("OSName", str_OSFullName)
                xml_LogFile.WriteEndElement()
                xml_LogFile.WriteStartElement("Options")
                Select Case str_MigrationType.ToUpper
                    Case "SCANSTATE"
                        xml_LogFile.WriteAttributeString("HealthCheck", bln_HealthCheck)
                        If bln_HealthCheck Then
                            xml_LogFile.WriteAttributeString("HealthCheckStatusOk", bln_HealthCheckStatusOk)
                        End If
                        xml_LogFile.WriteAttributeString("XPOnly", bln_MigrationXPOnly)
                        xml_LogFile.WriteAttributeString("AllUsers", bln_MigrationSettingsAllUsers)
                        xml_LogFile.WriteAttributeString("CompressionDisabled", bln_MigrationCompressionDisabled)
                End Select
                xml_LogFile.WriteAttributeString("EncryptionDisabled", bln_MigrationEncryptionDisabled)
                If Not bln_MigrationEncryptionDisabled Then
                    xml_LogFile.WriteAttributeString("EncryptionCustom", bln_MigrationEncryptionCustom)
                End If
                xml_LogFile.WriteEndElement()
                xml_LogFile.WriteStartElement(str_MigrationType)
                Select Case str_MigrationType.ToUpper
                    Case "SCANSTATE"
                        xml_LogFile.WriteAttributeString("DataSize", class_Migration.EstDataSize)
                End Select
                xml_LogFile.WriteAttributeString("TimeStart", dtm_StartTime)
                xml_LogFile.WriteAttributeString("TimeEnd", DateTime.Now)
                xml_LogFile.WriteAttributeString("StatusOk", bln_MigrationStatusOk)
                xml_LogFile.WriteAttributeString("ExitCode", class_Migration.ExitCode)
                xml_LogFile.WriteEndElement()
                xml_LogFile.Close()
            Catch ex As Exception
                sub_DebugMessage("ERROR: Failed to build XML log file: " & ex.Message, True)
            End Try

            ' Send Email
            If bln_MailSend And Not bln_MigrationCancelled Then
                sub_DebugMessage("Emailing results...")
                ' Check for Network connectivity
                sub_DebugMessage("Checking for network connection...")
                If Not My.Computer.Network.IsAvailable Then
                    sub_DebugMessage("No internet connection available. Skipping...")
                Else

                    Try
                        Dim email As New classEmail
                        email.Server = str_MailServer
                        email.Recipients = str_MailRecipients
                        email.From = str_MailFrom

                        If bln_MigrationStatusOk Then
                            email.Subject = "MigAssistant Success - " & str_MigrationType & ": " & str_EnvUserName & " (" & str_EnvComputerName & ")"
                        Else
                            email.Subject = "MigAsssitant Failure - " & str_MigrationType & ": " & str_EnvUserName & " (" & str_EnvComputerName & ")"
                        End If

                        email.Message = My.Resources.mailMessage

                        ' Add attachments
                        Dim attachmentArray As New ArrayList
                        ' Add WMA XML Logfiles if found
                        If My.Computer.FileSystem.FileExists(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder) & "\WMA_" & str_MigrationType & ".XML") Then
                            attachmentArray.Add(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder & "\WMA_" & str_MigrationType & ".XML"))
                        End If
                        ' Add Migration Logfiles if found
                        If My.Computer.FileSystem.FileExists(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder) & "\" & str_MigrationType & ".Log") Then
                            attachmentArray.Add(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder & "\" & str_MigrationType & ".Log"))
                        End If
                        ' Add Migration Progress Logfiles if found
                        If My.Computer.FileSystem.FileExists(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder) & "_Progress.Log") Then
                            attachmentArray.Add(Path.Combine(str_MigrationFolder, str_MigrationLoggingFolder & "_Progress.Log"))
                        End If
                        ' Add the debug Logfile if found (by generating a copy)
                        If My.Computer.FileSystem.FileExists(str_LogFile) Then
                            My.Computer.FileSystem.CopyFile(str_LogFile, str_LogFile & "_.Log", True)
                            attachmentArray.Add(str_LogFile.Trim & "_.Log")
                        End If
                        If Not attachmentArray.Count = 0 Then
                            email.Attachments = Join(attachmentArray.ToArray, ",")
                        End If

                        ' Update Migration status text
                        label_MigrationCurrentPhase.Text = "Emailing Results..."
                        Application.DoEvents()

                        ' Send Email
                        email.Send()
                        sub_DebugMessage("Email Sent")
                    Catch ex As Exception
                        sub_DebugMessage("ERROR: Email Send failed: " & ex.Message, True)
                    End Try

                End If

            End If

            ' Update Migration status text
            If bln_MigrationStatusOk Then
                label_MigrationCurrentPhase.ForeColor = Color.Green
                label_MigrationCurrentPhase.Text = My.Resources.migrationSuccessStatus
                If bln_AppProgressOnlyMode Then
                    appShutdown(My.Resources.exitCodeOk)
                End If
            Else
                label_MigrationCurrentPhase.ForeColor = Color.Red
                label_MigrationCurrentPhase.Text = My.Resources.migrationFailedStatus
                sub_DebugMessage(My.Resources.migrationFailedMessage)
                If bln_AppProgressOnlyMode Then
                    appShutdown(My.Resources.exitCodeMigrationFailed)
                End If
            End If

            bln_MigrationInProgress = False

            sub_SupportFormControlsReset()

        End If

    End Sub

    Private Sub sub_SupportFormControlsReset()

        sub_DebugMessage()
        sub_DebugMessage("* Form Controls Reset *")

        ' Focus the application and reset the in progress items
        Me.TopMost = True : Me.TopMost = False

        ' If cancelled, update Status
        If bln_MigrationCancelled Then
            label_MigrationCurrentPhase.ForeColor = Color.Red
            label_MigrationCurrentPhase.Text = "Migration Cancelled"
        End If

        ' Disable / Update status items
        progressbar_Migration.Visible = False
        label_MigrationEstTimeRemaining.Text = "..."
        label_MigrationEstSize.Text = "..."

        ' Enable / Update form controls
        button_AdvancedSettings.Enabled = True
        tabcontrol_MigrationType.Enabled = True
        button_Start.Text = "&Start"

        ' Remove unwanted text
        label_MigrationEstSize.Text = Nothing
        label_MigrationEstTimeRemaining.Text = Nothing

    End Sub

    Private Sub sub_MigrationProgressMonitor() Handles class_Migration.ProgressUpdate

        Try

            ' Handle thread invoking so that the UI can be updated cross-thread
            If Me.InvokeRequired Then
                Me.Invoke(New MethodInvoker(AddressOf sub_MigrationProgressMonitor))
            Else

                ' Update form from class information
                If Not class_Migration.EstTimeRemaining = 0 Then
                    label_MigrationEstTimeRemaining.Text = class_Migration.EstTimeRemaining & " " & My.Resources.migrationTotalMinutesRemaining
                End If
                If Not class_Migration.EstDataSize = 0 Then
                    label_MigrationEstSize.Text = class_Migration.EstDataSize & My.Resources.migrationTotalSizeInMBToTransfer
                    ' Check if a ScanState is being performed
                    If str_MigrationType = "SCANSTATE" Then
                        If Not bln_SizeChecksDone Then
                            ' If not overridden via commandline, check the migration size is not exceeded
                            If (Not bln_migrationMaxOverride Or int_MigrationMaxSize = 0) And class_Migration.EstDataSize > int_MigrationMaxSize Then
                                Throw New Exception("ERROR: The amount of data to be migrated exceeds the maximum allowed size. Please remove any unnecessary data and try again." & vbNewLine & vbNewLine & "Current: " & class_Migration.EstDataSize & "MB" & vbNewLine & "Maximum: " & int_MigrationMaxSize & "MB")
                            End If
                            If class_Migration.EstDataSize > func_GetFreeSpace(str_MigrationFolder) Then
                                Throw New Exception("ERROR: There is not enough space available to perform the migration. Estimated Migration Size: " & class_Migration.EstDataSize & "MB. Available Space: " & func_GetFreeSpace(str_MigrationFolder) & "MB")
                            End If
                            bln_SizeChecksDone = True
                        End If
                    End If
                End If
                ' Get current status
                If Not class_Migration.PercentComplete = 0 Then
                    str_StatusMessage = class_Migration.Progress & " (" & class_Migration.PercentComplete & My.Resources.migrationTotalPercentageCompleted & ")"
                Else
                    str_StatusMessage = class_Migration.Progress
                End If

                ' If status has changed, Output
                If Not str_StatusMessage = str_PreviousStatusMessage Then
                    sub_DebugMessage(str_StatusMessage)
                    label_MigrationCurrentPhase.Text = str_StatusMessage
                    progressbar_Migration.Value = class_Migration.PercentComplete
                End If

                str_PreviousStatusMessage = str_StatusMessage

            End If

        Catch ex As Exception
            class_Migration.SpinDown()
            sub_DebugMessage(ex.Message, True)
            label_MigrationCurrentPhase.Text = ex.Message
            Exit Sub
        End Try

    End Sub

    Private Sub sub_MigrationCreateFolderStructure(ByVal folderStructure As String)
        Dim array_Folder() As String
        Dim str_FolderBuild As String = Nothing
        Dim i As Integer

        If Not My.Computer.FileSystem.DirectoryExists(folderStructure) Then
            ' Replace UNC path so the array can be built correctly
            folderStructure = Replace(folderStructure, "\\", "//")
            array_Folder = Split(folderStructure, "\")
            For i = 0 To UBound(array_Folder)
                If str_FolderBuild = Nothing Then
                    str_FolderBuild = array_Folder(i)
                Else
                    str_FolderBuild = Path.Combine(str_FolderBuild, array_Folder(i))
                End If
                ' Reinstate UNC path
                str_FolderBuild = Replace(folderStructure, "//", "\\")
                Try
                    If Not My.Computer.FileSystem.DirectoryExists(str_FolderBuild) Then
                        My.Computer.FileSystem.CreateDirectory(str_FolderBuild)
                    End If
                Catch exIO As IOException
                    sub_DebugMessage("ERROR: " & exIO.Message, True)
                Catch ex As Exception
                    sub_DebugMessage("ERROR: " & ex.Message, True)
                End Try
            Next
        End If
    End Sub

    Private Sub sub_MigrationFindDataStore()

        Dim str_TempMigrationFolder As String = Nothing
        Dim array_TempFolder() As String = Nothing

        sub_DebugMessage("* Find Migration Data Store *")

        ' Check that WMA is configured correctly
        If Not func_InitCheckForValidConfiguration() Then
            sub_DebugMessage("ERROR: It looks like you haven't yet modified the WMA configuration file (MigAssistant.Exe.Config). As a result, it is not possible to perform a restore!", True)
            tabcontrol_MigrationType.SelectedTab = tabpage_Capture
            Exit Sub
        End If

        ' *** Check where the Restore is being performed from...
        Try

            If bln_MigrationLocationUseOther Then
                sub_DebugMessage("Migrating from Alternate location: " & str_MigrationLocationOther)
                str_MigrationFolder = str_MigrationLocationOther
            ElseIf bln_MigrationLocationUseUSB Then
                sub_DebugMessage("Migrating from USB drive: " & str_MigrationLocationUSB)
                str_MigrationFolder = str_MigrationLocationUSB
            ElseIf str_MigrationLocationNetwork.Contains("\\") Then
                sub_DebugMessage("Migrating from Network Location: " & str_MigrationLocationNetwork)
                Dim str_migrationServer As String = str_MigrationLocationNetwork.Replace("\\", "")
                str_migrationServer = str_migrationServer.Remove(str_migrationServer.IndexOf("\"))
                ' If no network connection available, or unable to ping server
                If Not My.Computer.Network.IsAvailable Or Not My.Computer.Network.Ping(str_migrationServer) Then
                    sub_DebugMessage("WARNING: " & My.Resources.migrationFindServerFailText, True)
                Else
                    str_MigrationFolder = str_MigrationLocationNetwork
                End If
            Else
                str_MigrationFolder = str_MigrationLocationNetwork
            End If

        Catch ex As Exception

        End Try

        Try
            ' Check for username in all migration folders
            sub_DebugMessage("Checking for username in all migration folders...")
            Dim col_MigrationFolders As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = My.Computer.FileSystem.GetDirectories(str_MigrationFolder)
            Dim arraylist_Folders As New ArrayList
            arraylist_Folders.Clear()
            For Each str_FolderSearch As String In col_MigrationFolders
                'folder = Replace(folder, str_MigrationFolder & "\", "")
                str_FolderSearch = Replace(str_FolderSearch, str_MigrationFolder & "\", "", 1, -1, CompareMethod.Text)
                If InStr(str_FolderSearch, str_EnvUserName, CompareMethod.Text) Then
                    arraylist_Folders.Add(str_FolderSearch)
                    sub_DebugMessage("Match Found: " & str_FolderSearch)
                End If
            Next
            Select Case arraylist_Folders.Count
                ' If no match
                Case 0
                    Throw New Exception(My.Resources.datastoreNotFound)
                    ' If one match
                Case 1
                    str_TempMigrationFolder = arraylist_Folders(0)
                    ' If multiple matches
                Case Else
                    ' Present option to select the correct datastore
                    formRestoreMultiDatastore.cbxRestoreMultiDatastoreList.Items.Clear()
                    For Each folderTmp As String In arraylist_Folders
                        formRestoreMultiDatastore.cbxRestoreMultiDatastoreList.Items.Add(folderTmp)
                    Next
                    formRestoreMultiDatastore.cbxRestoreMultiDatastoreList.SelectedItem = formRestoreMultiDatastore.cbxRestoreMultiDatastoreList.Items(0)
                    formRestoreMultiDatastore.ShowDialog()
                    str_TempMigrationFolder = formRestoreMultiDatastore.cbxRestoreMultiDatastoreList.SelectedItem
                    If str_TempMigrationFolder = Nothing Then
                        Throw New Exception(My.Resources.datastoreMultipleFoundNoneSelected)
                    End If
            End Select
        Catch ex As Exception
            sub_DebugMessage("ERROR: " & My.Resources.datastoreDetectionError & ": " & ex.Message, True)
            Exit Sub
        End Try

        str_MigrationFolder = Path.Combine(str_MigrationFolder, str_TempMigrationFolder).Trim

        sub_DebugMessage("Migration DataStore Full Path: " & str_MigrationFolder)

        label_DatastoreLocation.Text = str_MigrationFolder

        Try
            Dim xml_Document As XmlDocument = New XmlDocument
            xml_Document.Load(Path.Combine(str_MigrationFolder, "Logging\WMA_ScanState.XML"))
            Dim xml_Nodes As XmlNodeList = xml_Document.GetElementsByTagName("MigAssistant")
            xml_Document = Nothing
            For Each Xml_Node As XmlNode In xml_Nodes
                bln_MigrationSettingsAllUsers = Xml_Node.Item("Options").Attributes.GetNamedItem("AllUsers").Value
                bln_MigrationCompressionDisabled = Xml_Node.Item("Options").Attributes.GetNamedItem("CompressionDisabled").Value
                bln_MigrationEncryptionDisabled = Xml_Node.Item("Options").Attributes.GetNamedItem("EncryptionDisabled").Value
                If Not bln_MigrationEncryptionDisabled Then
                    bln_MigrationEncryptionCustom = Xml_Node.Item("Options").Attributes.GetNamedItem("EncryptionCustom").Value
                    If bln_MigrationEncryptionCustom Then
                        formCustomEncryption.ShowDialog()
                    End If
                End If
                bln_MigrationXPOnly = Xml_Node.Item("Options").Attributes.GetNamedItem("XPOnly").Value
                If bln_MigrationXPOnly And dbl_OSVersion >= 6.0 Then
                    Throw New Exception("Capture was performed using XP Only Mode but OS is not XP! Unable to perform migration")
                End If
                Dim int_MigrationDataSize As Integer = Xml_Node.Item("SCANSTATE").Attributes.GetNamedItem("DataSize").Value
                If str_PrimaryDataDrive = Nothing Then str_PrimaryDataDrive = "C:"
                If int_MigrationDataSize > func_GetFreeSpace(str_PrimaryDataDrive) Then
                    Throw New Exception("There is not enough free space on this drive to perform the migration")
                End If
                Exit For
            Next
        Catch ex As Exception
            sub_DebugMessage("ERROR: While parsing WMA_SCANSTATE.XML: " & ex.Message, True)
            appShutdown(30)
        End Try

    End Sub

    Private Function func_MigrationSupportScripts(ByVal migrationTimeLine As String) As Boolean
        arraylist_ScriptsCurrent.Clear()

        Select Case str_MigrationType
            Case "SCANSTATE"
                Select Case migrationTimeLine.ToUpper
                    Case "PRE"
                        For Each script As String In array_MigrationScriptsPreCapture
                            If Not script.Trim = Nothing Then
                                arraylist_ScriptsCurrent.Add(script)
                            End If
                        Next
                    Case "POST"
                        For Each script As String In array_MigrationScriptsPostCapture
                            If Not script.Trim = Nothing Then
                                arraylist_ScriptsCurrent.Add(script)
                            End If
                        Next
                End Select
            Case "LOADSTATE"
                Select Case migrationTimeLine.ToUpper
                    Case "PRE"
                        For Each script As String In array_MigrationScriptsPreRestore
                            If Not script.Trim = Nothing Then
                                arraylist_ScriptsCurrent.Add(script)
                            End If
                        Next
                    Case "POST"
                        For Each script As String In array_MigrationScriptsPostRestore
                            If Not script.Trim = Nothing Then
                                arraylist_ScriptsCurrent.Add(script)
                            End If
                        Next
                End Select
        End Select

        ' If no scripts to run, then exit the Function
        If arraylist_ScriptsCurrent.Count = 0 Then
            sub_DebugMessage("No scripts to run...")
            Return True
        End If

        ' Otherwise, process each script and return if one of them fails
        For Each script As String In arraylist_ScriptsCurrent
            sub_DebugMessage(migrationTimeLine & "-Migration Script: " & script.Remove(script.Length - 4) & "...")
            label_MigrationCurrentPhase.Text = migrationTimeLine & "-Migration Script: " & script.Remove(script.Length - 4) & "..."
            Application.DoEvents()
            Dim process As New Process
            Dim processInfo As New ProcessStartInfo()
            Dim str_TempFileCheck As String = Nothing
            Dim str_MigrationScriptArguments As String = _
                    "/USER " & str_EnvUserName & " " & _
                    "/COMPUTER " & str_EnvComputerName & " " & _
                    "/MIGFOLDER " & str_MigrationFolder & " "
            str_TempFileCheck = script.Substring(script.Length - 3)
            Select Case str_TempFileCheck.ToUpper
                Case "VBS", "VBE"
                    processInfo.FileName = "CScript.Exe"
                    processInfo.Arguments = """" & script & """ " & str_MigrationScriptArguments
                    processInfo.CreateNoWindow = bln_MigrationScriptsNoWindow
                Case Else
                    processInfo.FileName = script
                    processInfo.Arguments = str_MigrationScriptArguments
                    processInfo.CreateNoWindow = bln_MigrationScriptsNoWindow
            End Select
            ' Configure the Migration process
            processInfo.WorkingDirectory = str_WMAFolder
            processInfo.UseShellExecute = False

            ' Log debugging info
            sub_DebugMessage("-- " & processInfo.FileName & " " & processInfo.Arguments)

            ' Start the Migration Process
            Try
                process.StartInfo = processInfo
                process.Start()
                process.WaitForExit()
            Catch ex As Exception
                sub_DebugMessage("ERROR: " & ex.Message, True)
            End Try

            Select Case process.ExitCode
                Case 0
                    Return True
                Case Else
                    Return False
            End Select
        Next

    End Function

#End Region

End Class
