Imports Microsoft.VisualBasic
Imports System.net.Mail

Public Class classEmail
    Private _str_MailServer As String
    Private _str_MailRecipients As String
    Private _str_MailFrom As String
    Private _str_MailSubject As String
    Private _str_MailMessage As String
    Private _str_MailAttachments As String

    Public Property Server() As String
        Get
            Return _str_MailServer
        End Get
        Set(ByVal value As String)
            _str_MailServer = value
        End Set
    End Property
    Public Property Recipients() As String
        Get
            Return _str_MailRecipients
        End Get
        Set(ByVal value As String)
            _str_MailRecipients = value
        End Set
    End Property
    Public Property From() As String
        Get
            Return _str_MailFrom
        End Get
        Set(ByVal value As String)
            _str_MailFrom = value
        End Set
    End Property
    Public Property Subject() As String
        Get
            Return _str_MailSubject
        End Get
        Set(ByVal value As String)
            _str_MailSubject = value
        End Set
    End Property
    Public Property Message() As String
        Get
            Return _str_MailMessage
        End Get
        Set(ByVal value As String)
            _str_MailMessage = value
        End Set
    End Property
    Public Property Attachments() As String
        Get
            Return _str_MailAttachments
        End Get
        Set(ByVal value As String)
            _str_MailAttachments = value
        End Set
    End Property
    Public Sub Send()
        'This procedure takes string array parameters for multiple recipients and files
        Try
            'For each to address create a mail message
            Dim Message As New MailMessage
            Message.BodyEncoding = System.Text.Encoding.Default
            Message.Subject = _str_MailSubject.Trim()
            Message.Body = _str_MailMessage.Trim() & vbCrLf
            Message.From = New MailAddress(_str_MailFrom.Trim())
            Message.Priority = MailPriority.Normal
            Message.IsBodyHtml = True

            For Each recipient As String In Split(_str_MailRecipients, ",")
                Message.To.Add(New MailAddress(recipient.Trim()))
            Next

            'attach each file attachment
            For Each attachment As String In Split(_str_MailAttachments, ",")
                If Not attachment = "" Or Nothing Then
                    Dim MsgAttach As New Attachment(attachment)
                    Message.Attachments.Add(MsgAttach)
                End If
            Next

            'Smtpclient to send the mail message
            Dim SmtpMail As New SmtpClient
            SmtpMail.Host = _str_MailServer
            SmtpMail.Send(Message)
            'Message Successful
        Catch exSMTPFailedRecipients As SmtpFailedRecipientsException
            Throw New Exception(exSMTPFailedRecipients.Message)
        Catch exSMTPFailedRecipient As SmtpFailedRecipientException
            Throw New Exception(exSMTPFailedRecipient.Message)
        Catch exSMTP As SmtpException
            Throw New Exception(exSMTP.Message)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

End Class
