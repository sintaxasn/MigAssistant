Public Class formCustomEncryption

    Private Sub btnAdvancedSettingsClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomEncryptionClose.Click

        Me.Close()

    End Sub

    Public Sub form_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

        Select Case str_MigrationType
            Case "SCANSTATE"
                If Not formMigrationAdvancedSettings.rbnAdvancedSettingsQuestion2A.Checked Then
                    If tbxCustomEncryptionKey1.Text.Length = 0 And tbxCustomEncryptionKey2.Text.Length = 0 Then
                        MsgBox(My.Resources.encryptionNoKeySpecified, MsgBoxStyle.Information, My.Resources.appTitle)
                        formMigrationAdvancedSettings.rbnAdvancedSettingsQuestion2A.Checked = True
                    ElseIf tbxCustomEncryptionKey1.Text <> tbxCustomEncryptionKey2.Text Then
                        MsgBox(My.Resources.encryptionKeysDontMatch, MsgBoxStyle.Exclamation, My.Resources.appTitle)
                        tbxCustomEncryptionKey1.Text = Nothing
                        tbxCustomEncryptionKey2.Text = Nothing
                        tbxCustomEncryptionKey1.Focus()
                        e.Cancel = True
                        Exit Sub
                    End If
                    str_customEncryptionKey = tbxCustomEncryptionKey1.Text

                    ' Stop the form from actually closing, and hide instead
                    e.Cancel = True
                    Me.Hide()
                End If
            Case "LOADSTATE"
                If tbxCustomEncryptionKey1.Text.Length = 0 And tbxCustomEncryptionKey2.Text.Length = 0 Then
                    e.Cancel = True
                    Exit Sub
                ElseIf tbxCustomEncryptionKey1.Text <> tbxCustomEncryptionKey2.Text Then
                    MsgBox(My.Resources.encryptionKeysDontMatch, MsgBoxStyle.Exclamation, My.Resources.appTitle)
                    tbxCustomEncryptionKey1.Text = Nothing
                    tbxCustomEncryptionKey2.Text = Nothing
                    tbxCustomEncryptionKey1.Focus()
                    e.Cancel = True
                    Exit Sub
                End If
                str_customEncryptionKey = tbxCustomEncryptionKey1.Text

                ' Stop the form from actually closing, and hide instead
                e.Cancel = True
                Me.Hide()

        End Select

    End Sub
End Class