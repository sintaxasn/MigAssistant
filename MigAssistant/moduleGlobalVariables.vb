Module moduleGlobalVariables

    ' P/Invokes
    Private Declare Function GetDiskFreeSpaceEx Lib "kernel32" Alias "GetDiskFreeSpaceExA" (ByVal lpDirectoryName As String, ByRef lpFreeBytesAvailableToCaller As Long, ByRef lpTotalNumberOfBytes As Long, ByRef lpTotalNumberOfFreeBytes As Long) As Long

    ' Constants
    Public Const str_MigrationDataStoreFolder As String = "Datastore"
    Public Const str_MigrationLoggingFolder As String = "Logging"
    Public Const str_MigrationXMLConfigName As String = "Migration.XML"

    ' Locale Settings
    Public strLocaleDecimal As String = Mid(CStr(11 / 10), 2, 1)
    Public strLocaleComma As String = Chr(90 - Asc(strLocaleDecimal))

    ' Set up encryption type
    Public encryption_SymmetricEncryption As New Encryption.Symmetric(Encryption.Symmetric.Provider.TripleDES)
    ' Set encryption key
    Public encryption_DataHash As New Encryption.Data("1kb3n33nb3st")

    ' Get OS Information
    Public dbl_OSVersion As Double = Left(My.Computer.Info.OSVersion, 3).Replace(".", strLocaleDecimal)
    Public str_OSFullName As String = My.Computer.Info.OSFullName
    Public str_OSArchitecture = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")

    ' Current Workstation Information
    Public str_EnvDomain As String = System.Environment.UserDomainName
    Public str_EnvUserName As String = Replace(My.User.CurrentPrincipal.Identity.Name, System.Environment.UserDomainName & "\", "", , , CompareMethod.Text)
    Public str_EnvComputerName As String = My.Computer.Name

    ' Set Up Variables
    Public arraylist_MigrationArguments As New ArrayList
    Public arraylist_ScriptsCurrent As New ArrayList
    Public bln_HealthCheck As Boolean = False
    Public bln_HealthCheckStatusOk As Boolean = False
    Public bln_MigrationStatusOk As Boolean = False
    Public bln_AppProgressOnlyMode As Boolean = False
    Public str_MigrationType As String = Nothing
    Public str_MigrationFolder As String = Nothing
    Public bln_MigrationCancelled As Boolean = False
    Public str_USMTGUID As String = Nothing
    Public obj_LogFile As System.IO.TextWriter
    Public str_PreviousStatusMessage As String = Nothing
    Public str_StatusMessage As String = Nothing
    Public dtm_StartTime As DateTime = Nothing
    Public bln_SizeChecksDone As Boolean = False
    Public bln_HealthCheckInProgress As Boolean = False
    Public bln_MigrationInProgress As Boolean = False
    Public bln_DownloadComplete As Boolean = False
    Public int_DownloadProgress As Integer = 0
    Public str_PrimaryDataDrive As String = Nothing

    ' Get Application Information
    Public str_USMTFolder As String = My.Computer.FileSystem.SpecialDirectories.ProgramFiles & "\USMT301"
    Public str_WMAFolder As String = My.Application.Info.DirectoryPath
    Public str_TempFolder As String = System.IO.Path.GetTempPath.TrimEnd("\")
    Public str_LogFile As String = My.Computer.FileSystem.SpecialDirectories.Temp & "\WMA.Log"
    Public str_WMAConfigNetworkCheck As String = "\\ServerName\MigrationShare"

    ' Get Settings from .Exe.Settings file
    Public str_MigrationConfigFile As String = My.Settings.MigrationConfig
    Public str_MigrationConfigFileXP As String = My.Settings.MigrationConfigXPOnly
    Public array_MigrationExclusionsDomain() As String = Split(My.Settings.MigrationExclusionsDomain, ",")
    Public array_MigrationExclusionsLocal() As String = Split(My.Settings.MigrationExclusionsLocal, ",")
    Public bln_MigrationMultiUserMode As Boolean = My.Settings.MigrationMultiUserMode
    Public int_MigrationExclusionsOlderThanDays As Integer = My.Settings.MigrationExclusionsOlderThanDays
    Public int_MigrationUSMTLoggingType As Integer = My.Settings.USMTLoggingValue
    Public str_MigrationLocationNetwork As String = My.Settings.MigrationNetworkLocation
    Public bln_MigrationLocationNetworkDisabled As Boolean = My.Settings.MigrationNetworkLocationDisabled
    Public int_MigrationMaxSize As Integer = My.Settings.MigrationMaxSize
    Public bln_MigrationEncryptionDisabled As Boolean = My.Settings.MigrationEncryptionDisabled
    Public str_MigrationEncryptionDefaultKey As String = My.Settings.MigrationEncryptionDefaultKey
    Public str_MigrationRestoreAccountsPassword As String = My.Settings.MigrationRestoreAccountsPassword
    Public bln_MigrationRestoreAccountsEnabled As Boolean = My.Settings.MigrationRestoreAccountsEnabled
    Public bln_SettingsAdvancedSettingsDisabled As Boolean = My.Settings.SettingsAdvancedSettingsDisabled
    Public bln_SettingsHealthCheckDefaultEnabled As Boolean = My.Settings.SettingsHealthCheckDefaultEnabled
    Public bln_SettingsWorkstationDetailsDisabled As Boolean = My.Settings.SettingsWorkstationDetailsDisabled
    Public bln_SettingsDebugMode As Boolean = My.Settings.SettingsDebugMode
    Public array_MigrationRuleSet() As String = Split(My.Settings.MigrationRuleSet, ",")
    Public array_MigrationRuleSetXPOnly() As String = Split(My.Settings.MigrationRuleSet, ",")
    Public bln_MigrationXPOnly As Boolean = My.Settings.MigrationXPOnly
    Public int_MigrationMinUSBDiskSize As Integer = My.Settings.MigrationUSBMinSize
    Public bln_MigrationUSBAutoUseIfAvailable As Boolean = My.Settings.MigrationUSBAutoUseIfAvailable
    Public bln_MigrationCompressionDisabled As Boolean = My.Settings.MigrationCompressionDisabled
    Public str_MigrationDomainChange As String = My.Settings.MigrationDomainChange
    Public array_MigrationScriptsPreCapture() As String = Split(My.Settings.MigrationScriptsPreCapture, ",")
    Public array_MigrationScriptsPostCapture() As String = Split(My.Settings.MigrationScriptsPostCapture, ",")
    Public array_MigrationScriptsPreRestore() As String = Split(My.Settings.MigrationScriptsPreRestore, ",")
    Public array_MigrationScriptsPostRestore() As String = Split(My.Settings.MigrationScriptsPostRestore, ",")
    Public bln_MigrationScriptsNoWindow As Boolean = My.Settings.MigrationScriptsNoWindow
    Public bln_MigrationOverwriteExistingFolders As Boolean = My.Settings.MigrationOverWriteExistingFolders
    Public bln_MailSend As Boolean = My.Settings.MailSend
    Public str_MailServer As String = My.Settings.MailServer
    Public str_MailRecipients As String = My.Settings.MailRecipients
    Public str_MailFrom As String = My.Settings.MailFrom

    ' Resources
    Public str_bddManifestURL As String = My.Resources.bddManifestURL
    Public str_bddManifestFile As String = My.Resources.bddManifestFile
    Public str_USMTGUIDx86 As String = My.Resources.usmtGUIDx86
    Public str_USMTGUIDx64 As String = My.Resources.usmtGUIDx64

    ' *** Migration Settings
    Public bln_MigrationSettingsAllUsers As Boolean = False
    Public bln_MigrationSettingsLocalAccounts As Boolean = False
    Public bln_MigrationLocationUseOther As Boolean = False
    Public str_MigrationLocationOther As String = Nothing
    Public bln_MigrationLocationUseUSB As Boolean = False
    Public str_MigrationLocationUSB As String = Nothing
    Public bln_MigrationEncryptionCustom As Boolean = False
    Public str_customEncryptionKey As String = 0
    Public bln_migrationMaxOverride As Boolean = False
    Public bln_migrationFolderOverride As Boolean = False

    Public Sub sub_DebugMessage(Optional ByVal str_DebugMessage As String = Nothing, Optional ByVal bln_DisplayError As Boolean = False, Optional ByVal bln_WriteEventLogEntry As Boolean = False)

        If bln_DisplayError = True Then
            If str_DebugMessage.Contains("INFO:") Then
                MsgBox(str_DebugMessage, MsgBoxStyle.Information + MsgBoxStyle.OkOnly + MsgBoxStyle.MsgBoxSetForeground, My.Resources.appTitle)
            ElseIf str_DebugMessage.Contains("WARNING:") Then
                MsgBox(str_DebugMessage, MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly + MsgBoxStyle.MsgBoxSetForeground, My.Resources.appTitle)
            ElseIf str_DebugMessage.Contains("ERROR:") Then
                MsgBox(str_DebugMessage, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly + MsgBoxStyle.MsgBoxSetForeground, My.Resources.appTitle)
            End If
        End If

        If bln_WriteEventLogEntry Then
            My.Application.Log.WriteEntry(str_DebugMessage)
        End If

        str_DebugMessage = "[" & DateTime.Now & "] " & str_DebugMessage

        If bln_SettingsDebugMode And Not obj_LogFile Is Nothing Then
            obj_LogFile.WriteLine(str_DebugMessage)
            obj_LogFile.Flush()
        Else
            Exit Sub
        End If

        Console.WriteLine(str_DebugMessage)

    End Sub

    Public Sub appInitialise()

        sub_DebugMessage()
        sub_DebugMessage("* Application Initialisation *")

        ' Create the logfile if in Debug mode, otherwise, just output to the console...
        If bln_SettingsDebugMode Then
            sub_DebugMessage("Running in Debug Mode")
            Try
                ' Check if the logfile already exists. If yes, delete it
                If My.Computer.FileSystem.FileExists(str_LogFile) Then
                    sub_DebugMessage("Logfile already exists. Attempting to delete...")
                    Try
                        My.Computer.FileSystem.DeleteFile(str_LogFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                        sub_DebugMessage("Logfile deleted")
                    Catch ex As Exception
                        Throw New Exception(ex.Message)
                    End Try
                End If

                ' Connect to the logfile
                Try
                    sub_DebugMessage("Initialising logfile...")
                    obj_LogFile = My.Computer.FileSystem.OpenTextFileWriter(str_LogFile, True)
                Catch ex As Exception
                    Throw New Exception(ex.Message)
                End Try

            Catch ex As Exception
                MessageBox.Show("ERROR: Unable to create Debug log file: " & ex.Message & ". Debugging switched off", My.Resources.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                bln_SettingsDebugMode = False
            End Try
        End If

        sub_DebugMessage(My.Resources.appTitle & " " & My.Resources.appBuild & " - " & My.Resources.appCompany)

    End Sub

    Public Sub appShutdown(ByVal int_ExitCode As Integer)

        sub_DebugMessage()
        sub_DebugMessage("* Application Shutdown *")

        ' Close the logfile
        If bln_SettingsDebugMode Then
            sub_DebugMessage("Closing Logfile...")
            obj_LogFile.Close()
            obj_LogFile = Nothing
        End If

        sub_DebugMessage("Exiting Application with Exit Code: " & int_ExitCode, False, True)
        System.Environment.Exit(int_ExitCode)

    End Sub

    Public Function func_GetFreeSpace(ByVal str_Location As String) As Long

        Dim lng_BytesTotal, lng_FreeBytes, lng_FreeBytesAvailable, lng_Result As Long
        lng_Result = GetDiskFreeSpaceEx(str_Location, lng_FreeBytesAvailable, lng_BytesTotal, lng_FreeBytes)
        If lng_Result > 0 Then
            Return func_BytesToMB(lng_FreeBytes)
        Else
            Throw New Exception("ERROR: Invalid or unreadable location")
        End If

    End Function

    Private Function func_BytesToMB(ByVal lng_Bytes As Long) As Long

        Dim dbl_Result As Double
        dbl_Result = (lng_Bytes / 1024) / 1024
        func_BytesToMB = Format(dbl_Result, "###,###,##0.00")

    End Function
End Module
