Imports System.IO
Imports System.Threading

Public Class classMigration

#Region "Declarations"

    ' Event Declarations
    Private WithEvents _fsw_migrationProgressFileWatcher As New FileSystemWatcher

    ' Class Declarations
    Private _str_MigrationArguments As String = Nothing
    Private _arraylist_MigrationArguments As New ArrayList
    Private _str_MigrationProgress As String = Nothing
    Private _str_MigrationDebugInfo As String = Nothing
    Private _int_MigrationEstDataSize As Integer = 0
    Private _int_MigrationExitCode As Integer = 0
    Private _bln_MigrationInProgress As Boolean = False
    Private _str_MigrationLogFile As String = Nothing
    Private _int_MigrationEstTimeRemaining As Integer = 0
    Private _int_MigrationPercentComplete As Integer = 0
    Private _str_MigrationProgressFile As String = Nothing
    Private _io_MigrationProgressFileParser As Microsoft.VisualBasic.FileIO.TextFieldParser
    Private _int_MigrationProgressFileLastLineNumber As Integer = 0
    Private _bln_MigrationProgressCheckInProgress As Boolean = False
    Private _str_MigrationType As String = Nothing
    Private _thread As Thread
    Private _process As Process

#End Region

#Region "Properties"
    ' Property: Return the argument list for USMT
    Public Property Arguments() As ArrayList
        Get
            Return _arraylist_MigrationArguments
        End Get
        Set(ByVal value As ArrayList)
            _arraylist_MigrationArguments = value
        End Set
    End Property
    ' Property: Return the migration type
    Public Property Type() As String
        Get
            Return _str_MigrationType
        End Get
        Set(ByVal value As String)
            _str_MigrationType = value
        End Set
    End Property
    ' Property: Return whether the migration is in progress
    Public ReadOnly Property InProgress() As Boolean
        Get
            Return _bln_MigrationInProgress
        End Get
    End Property
    ' Property: Return the migration current progress
    Public ReadOnly Property Progress()
        Get
            Return _str_MigrationProgress
        End Get
    End Property
    ' Property: Return the migration exit code
    Public ReadOnly Property ExitCode()
        Get
            Return _int_MigrationExitCode
        End Get
    End Property
    ' Property: Return the migration current debug info
    Public ReadOnly Property DebugInfo()
        Get
            Return _str_MigrationDebugInfo
        End Get
    End Property
    ' Property: Return the migration data size
    Public ReadOnly Property EstDataSize()
        Get
            Return _int_MigrationEstDataSize
        End Get
    End Property
    ' Property: Return the migration minutes remaining
    Public ReadOnly Property EstTimeRemaining()
        Get
            Return _int_MigrationEstTimeRemaining
        End Get
    End Property
    ' Property: Return the migration percent complete
    Public ReadOnly Property PercentComplete()
        Get
            Return _int_MigrationPercentComplete
        End Get
    End Property
    ' Property: Return the path to the migration log file
    Public ReadOnly Property LogFile()
        Get
            Return str_MigrationFolder & "\" & str_MigrationLoggingFolder & "\" & _str_MigrationLogFile
        End Get
    End Property
#End Region

#Region "Events"

    Public Event ProgressUpdate()
    Public Event MigrationFinished()

#End Region

#Region "Subroutines"

    Private Sub Start()
        Try
            ' If the Migration Progress File exists, delete it
            If My.Computer.FileSystem.FileExists(str_MigrationFolder & "\" & _
                    str_MigrationLoggingFolder & "\" & _str_MigrationProgressFile) Then
                My.Computer.FileSystem.DeleteFile(str_MigrationFolder & "\" & _
                        str_MigrationLoggingFolder & "\" & _str_MigrationProgressFile)
            End If

            ' Add Progress and Logfile details to the argument list
            _arraylist_MigrationArguments.Add("/Progress:""" & str_MigrationFolder & "\" & str_MigrationLoggingFolder & "\" & _str_MigrationProgressFile & """")
            _arraylist_MigrationArguments.Add("/L:""" & str_MigrationFolder & "\" & str_MigrationLoggingFolder & "\" & _str_MigrationLogFile & """")
            ' Sort and Dump the arguments from the array into a string containing spaces
            _arraylist_MigrationArguments.Sort()
            _str_MigrationArguments = Join(_arraylist_MigrationArguments.ToArray, " ")

            ' Setup and configure the new process
            _process = New Process
            Dim _processInfo As New ProcessStartInfo(str_USMTFolder & "\" & _
                    _str_MigrationType & ".Exe", _str_MigrationArguments)
            _processInfo.WorkingDirectory = str_USMTFolder
            _processInfo.UseShellExecute = True
            _processInfo.WindowStyle = ProcessWindowStyle.Hidden
            _process.StartInfo = _processInfo

            ' Start the Migration Process
            _process.Start()
            _process.WaitForExit()
            _int_MigrationExitCode = _process.ExitCode

            ' Prevent error for appearing when process is aborted and STDOut gets redirected
        Catch exInvalidOperation As InvalidOperationException
            sub_DebugMessage("WARNING: Migration has been cancelled.")
            ' Return Cancelled Error Code
            _int_MigrationExitCode = 999
        Catch ex As Exception
            MsgBox("An error occurred during the Migration:" & vbNewLine & vbNewLine & "Error: " & ex.Message, MsgBoxStyle.Critical, My.Resources.appTitle)
        Finally
            sub_DebugMessage("RaiseEvent: MigrationFinished")
            RaiseEvent MigrationFinished()
        End Try

    End Sub
    Public Sub Spinup()
        ' Reset Variables
        _str_MigrationProgress = Nothing
        _str_MigrationDebugInfo = Nothing
        _int_MigrationEstDataSize = 0
        _int_MigrationEstTimeRemaining = 0
        _int_MigrationExitCode = 0
        _int_MigrationPercentComplete = 0
        _int_MigrationProgressFileLastLineNumber = 0
        _str_MigrationLogFile = _str_MigrationType & ".Log"
        _str_MigrationProgressFile = str_MigrationType & "_Progress.Log"

        ' Set up the Progress File watcher
        With _fsw_migrationProgressFileWatcher
            .Path = str_MigrationFolder & "\" & str_MigrationLoggingFolder
            .Filter = _str_MigrationProgressFile
            .IncludeSubdirectories = False
            .EnableRaisingEvents = True
        End With

        ' Start the migration in a new thread
        _bln_MigrationInProgress = True

        Dim threadStart As New ThreadStart(AddressOf Me.Start)
        _thread = New Thread(threadStart)
        _thread.Start()

    End Sub

    Public Sub SpinDown()

        ' Cleanup
        sub_DebugMessage("Thread Cleanup...")
        _fsw_migrationProgressFileWatcher.EnableRaisingEvents = False
        _fsw_migrationProgressFileWatcher.Dispose()
        _bln_MigrationInProgress = False

        Try

            ' Ensure the process has terminated
            If Not _process.HasExited Then
                sub_DebugMessage("Terminating process...")
                _process.Kill()
            End If
            _process.Close()
            _thread = Nothing

        Catch ex As Exception

        End Try

    End Sub

    Private Sub _sub_MigrationProgressReturn(ByVal sender As Object, ByVal e As FileSystemEventArgs) Handles _fsw_migrationProgressFileWatcher.Changed

        ' If we're currently checking the progress, don't monitor it again
        ' sub_DebugMessage("DEBUG: Checking if progress-monitoring is already running...")
        If _bln_MigrationProgressCheckInProgress Then Exit Sub

        ' Sleep to ensure file writing is complete
        Thread.Sleep(500)

        ' Set up initial values
        Dim _array_MigrationProgressFileCurrentRow As String() = Nothing
        Dim _int_MigrationProgressFileCurrentLine As Integer = 0

        ' Read Progress File into the parser
        Try
            ' sub_DebugMessage("DEBUG: Reading Progress File: " & str_MigrationFolder & "\" & str_MigrationLoggingFolder & "\" & _str_MigrationProgressFile)
            _io_MigrationProgressFileParser = My.Computer.FileSystem.OpenTextFieldParser(str_MigrationFolder & "\" & _
                        str_MigrationLoggingFolder & "\" & _str_MigrationProgressFile)
            _io_MigrationProgressFileParser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
            _io_MigrationProgressFileParser.Delimiters = New String() {","}
            _io_MigrationProgressFileParser.HasFieldsEnclosedInQuotes = True
            _io_MigrationProgressFileParser.TrimWhiteSpace = True
        Catch ex As Exception
            ' sub_DebugMessage("DEBUG: EXCEPTION CAUGHT: " & ex.Message)
            ' Exit the sub if failed (ie, our file doesn't exist yet)
            Exit Sub
        End Try

        ' Go through each line in the log file
        While Not _io_MigrationProgressFileParser.EndOfData
            ' Make sure we exit the loop if the migration has been cancelled
            If Not _fsw_migrationProgressFileWatcher.EnableRaisingEvents Then
                sub_DebugMessage("Stopping progress monitoring...")
                Exit While
            End If
            ' sub_DebugMessage("DEBUG: File not finished")
            _bln_MigrationProgressCheckInProgress = True
            Try
                ' Read the current line into an array
                ' sub_DebugMessage("DEBUG: Reading current line...")
                _array_MigrationProgressFileCurrentRow = _io_MigrationProgressFileParser.ReadFields()
                ' sub_DebugMessage("DEBUG: " & Join(_array_MigrationProgressFileCurrentRow, ","))
                ' If this line number is higher than the last line, process it

                If UBound(_array_MigrationProgressFileCurrentRow) > 3 And _int_MigrationProgressFileCurrentLine > _int_MigrationProgressFileLastLineNumber Then
                    Select Case _array_MigrationProgressFileCurrentRow(3)
                        Case "PHASE"
                            Select Case _array_MigrationProgressFileCurrentRow(4)
                                Case "Initializing"
                                    _str_MigrationProgress = My.Resources.migrationPhaseInitializing
                                Case "Scanning"
                                    _str_MigrationProgress = My.Resources.migrationPhaseScanning
                                Case "Collecting"
                                    _str_MigrationProgress = My.Resources.migrationPhaseCollecting
                                Case "Saving"
                                    _str_MigrationProgress = My.Resources.migrationPhaseSaving
                                    _str_MigrationDebugInfo = Nothing
                                Case "Estimating"
                                    _str_MigrationProgress = My.Resources.migrationPhaseEstimating
                                Case "Applying"
                                    _str_MigrationProgress = My.Resources.migrationPhaseApplying
                            End Select
                        Case "totalSizeInMBToTransfer"
                            If Not _array_MigrationProgressFileCurrentRow(4) = "" Then _int_MigrationEstDataSize = _array_MigrationProgressFileCurrentRow(4).Replace(".", strLocaleDecimal)
                        Case "totalPercentageCompleted"
                            If Not _array_MigrationProgressFileCurrentRow(4) = "" Then _int_MigrationPercentComplete = _array_MigrationProgressFileCurrentRow(4).Replace(".", strLocaleDecimal)
                        Case "totalMinutesRemaining"
                            If Not _array_MigrationProgressFileCurrentRow(4) = "" Then _int_MigrationEstTimeRemaining = _array_MigrationProgressFileCurrentRow(4).Replace(".", strLocaleDecimal)
                        Case "detectedUser"
                            If _array_MigrationProgressFileCurrentRow(6) = "Yes" Then
                                _str_MigrationDebugInfo = My.Resources.migrationDetectedUser & " " & _array_MigrationProgressFileCurrentRow(4)
                            End If
                        Case "forUser"
                            If _array_MigrationProgressFileCurrentRow(8) = "Yes" Then
                                _str_MigrationDebugInfo = My.Resources.migrationForUser & " " & _array_MigrationProgressFileCurrentRow(4) & " - " & _array_MigrationProgressFileCurrentRow(6)
                            End If
                        Case "collectingUser"
                            _str_MigrationDebugInfo = My.Resources.migrationCollectingUser & " " & _array_MigrationProgressFileCurrentRow(4)
                        Case "errorCode"
                            Select Case _array_MigrationProgressFileCurrentRow(4)
                                Case 0
                                    _str_MigrationDebugInfo = My.Resources.migrationCompleteSuccess & " " & My.Resources.migrationNonFatalErrors & " " & _array_MigrationProgressFileCurrentRow(6)
                                Case Else
                                    _str_MigrationDebugInfo = My.Resources.migrationCompleteError & " " & _array_MigrationProgressFileCurrentRow(4)
                            End Select
                    End Select
                    ' Update the recorded line number
                    _int_MigrationProgressFileLastLineNumber = _int_MigrationProgressFileCurrentLine
                End If

            Catch ex As Microsoft.VisualBasic.FileIO.MalformedLineException
                sub_DebugMessage("Line " & _int_MigrationProgressFileCurrentLine & " is malformed and will be skipped: " & Join(_array_MigrationProgressFileCurrentRow, ", ") & " - " & ex.Message)
            Catch ex As Exception
                sub_DebugMessage("Error occurred while reading line " & _int_MigrationProgressFileCurrentLine & ": " & Join(_array_MigrationProgressFileCurrentRow, ", ") & " - " & ex.Message)
            Finally
                _int_MigrationProgressFileCurrentLine = _int_MigrationProgressFileCurrentLine + 1
                ' Raise event that the progress has changed
                If _bln_MigrationInProgress Then RaiseEvent ProgressUpdate()
            End Try
        End While
        _io_MigrationProgressFileParser.Close()
        _bln_MigrationProgressCheckInProgress = False
    End Sub
#End Region

End Class
