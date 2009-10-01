Imports System
Imports System.Threading

Public Class classHealthCheck

#Region "Declarations"

    Private _bln_HealthCheckInProgress As Boolean = False
    Private _int_HealthCheckExitCode As Integer = 0
    Private _int_HealthCheckPercentComplete As Integer = 0
    Private _str_HealthCheckProgress As String = Nothing
    Private _str_HealthCheckOutput As String = Nothing
    Private _thread As Thread
    Private _process As Process

#End Region

#Region "Properties"

    ' Property: Return Yes / No as to whether the scan is in progress
    Public ReadOnly Property InProgress() As Boolean
        Get
            Return _bln_HealthCheckInProgress
        End Get
    End Property
    Public ReadOnly Property ExitCode() As Integer
        Get
            Return _int_HealthCheckExitCode
        End Get
    End Property
    Public ReadOnly Property Progress() As String
        Get
            Return _str_HealthCheckProgress
        End Get
    End Property
    Public ReadOnly Property PercentComplete() As Integer
        Get
            Return _int_HealthCheckPercentComplete
        End Get
    End Property

#End Region

#Region "Events"

    Public Event ProgressUpdate()
    Public Event HealthCheckFinished()

#End Region

#Region "Subroutines"

    Private Sub Start()
        Try
            ' Set up and start the CHKDSK process
            _process = New Process
            Dim _processInfo As New ProcessStartInfo("CHKDSK", "C: /I /C")
            _processInfo.RedirectStandardError = True
            _processInfo.RedirectStandardOutput = True
            _processInfo.RedirectStandardInput = True
            _processInfo.CreateNoWindow = True
            _processInfo.UseShellExecute = False
            _process.StartInfo = _processInfo
            _process.Start()

            ' Continously check the output from CHKDSK and dump to a global variable
            Do While Not _process.StandardOutput.EndOfStream
                If _process.StandardOutput.ReadLine.Length <> 0 Then
                    _str_HealthCheckOutput = _process.StandardOutput.ReadLine
                    _sub_HealthCheckProgressReturn()
                End If
            Loop

            ' Wait until the process exits and check the error code
            _process.WaitForExit()

            ' Get the exit code
            _int_HealthCheckExitCode = _process.ExitCode

            ' Prevent error for appearing when process is aborted and STDOut gets redirected
        Catch exInvalidOperation As InvalidOperationException
            sub_DebugMessage("WARNING: Health Check has been cancelled.")
            ' Return Cancelled Error Code
            _int_HealthCheckExitCode = 999
        Catch ex As Exception
            sub_DebugMessage("ERROR: Heath Check Failed: " & ex.Message, True)
        Finally
            sub_DebugMessage("RaiseEvent: HealthCheckFinished")
            RaiseEvent HealthCheckFinished()
        End Try
    End Sub

    Public Sub Spinup()
        'Reset variables
        _str_HealthCheckOutput = Nothing
        _str_HealthCheckProgress = Nothing
        _int_HealthCheckExitCode = 0
        _int_HealthCheckPercentComplete = 0

        ' Start the scan in a new thread
        _bln_HealthCheckInProgress = True

        Dim ThreadStart As New ThreadStart(AddressOf Me.Start)
        _thread = New Thread(ThreadStart)
        _thread.Start()

    End Sub

    Public Sub SpinDown()

        ' Cleanup
        sub_DebugMessage("Thread Cleanup...")
        _bln_HealthCheckInProgress = False

        ' Ensure the process has terminated
        Try

            If Not _process.HasExited Then
                sub_DebugMessage("Terminating process...")
                _process.Kill()
            End If
            _process.Close()
            _thread = Nothing

        Catch ex As Exception

        End Try

    End Sub

    Private Sub _sub_HealthCheckProgressReturn()

        If Not _str_HealthCheckOutput = Nothing Then
            If _str_HealthCheckOutput.Contains("file records") Or _str_HealthCheckOutput.Contains("parameter specified") Then
                _int_HealthCheckPercentComplete = 0
                _str_HealthCheckProgress = My.Resources.diskScanStatus1
            ElseIf _str_HealthCheckOutput.Contains("index entries") Or _str_HealthCheckOutput.Contains("verifying indexes") Then
                _int_HealthCheckPercentComplete = 0
                _str_HealthCheckProgress = My.Resources.diskScanStatus2
            ElseIf _str_HealthCheckOutput.Contains("descriptors") Or _str_HealthCheckOutput.Contains("verification completed") Then
                _int_HealthCheckPercentComplete = 0
                _str_HealthCheckProgress = My.Resources.diskScanStatus3
            End If
            If _str_HealthCheckOutput.Contains("percent complete") Then
                _int_HealthCheckPercentComplete = _str_HealthCheckOutput.Substring(0, 2).Trim.Replace(".", strLocaleDecimal)
            End If
        End If

        ' Raise event that the progress has changed
        If _bln_HealthCheckInProgress Then RaiseEvent ProgressUpdate()

    End Sub

#End Region

End Class
