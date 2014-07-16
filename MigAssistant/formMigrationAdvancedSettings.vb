Public Class formMigrationAdvancedSettings

    Private Sub btnAdvancedSettingsClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAdvancedSettingsClose.Click

        ' Close Form
        Me.Close()

    End Sub

    Private Sub form_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

        If rbnAdvancedSettingsQuestion4B.Checked Then
            bln_MigrationSettingsLocalAccounts = True
        Else
            bln_MigrationSettingsLocalAccounts = False
        End If

        ' Stop the form from actually closing, and hide instead
        e.Cancel = True
        Me.Hide()

    End Sub

    Private Sub formMigrationAdvancedSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, MyBase.VisibleChanged
        ' If Encryption is disabled, hide on the settings page
        If bln_MigrationEncryptionDisabled Then
            lblAdvancedSettingsQuestion2.Visible = False
            rbnAdvancedSettingsQuestion2A.Visible = False
            rbnAdvancedSettingsQuestion2B.Visible = False
        End If

        lblAdvancedSettingsQuestion4.Visible = False
        rbnAdvancedSettingsQuestion4A.Visible = False
        rbnAdvancedSettingsQuestion4B.Visible = False

        ' If performing a backup...
        If str_MigrationType = "SCANSTATE" Then
            ' and migrating more than the current user, all local account migration too
            If bln_MigrationSettingsAllUsers Then
                lblAdvancedSettingsQuestion4.Visible = True
                rbnAdvancedSettingsQuestion4A.Visible = True
                rbnAdvancedSettingsQuestion4B.Visible = True
            End If
        End If
    End Sub

    Private Sub rbnAdvancedSettingsQuestion1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbnAdvancedSettingsQuestion1B.CheckedChanged, rbnAdvancedSettingsQuestion1A.CheckedChanged

        If rbnAdvancedSettingsQuestion1B.Checked Then
            If Not bln_MigrationLocationUseOther Then
                If fbdAdvancedSettingsDataStore.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                    bln_MigrationLocationUseOther = True
                    str_MigrationLocationOther = fbdAdvancedSettingsDataStore.SelectedPath
                Else
                    rbnAdvancedSettingsQuestion1A.Checked = True
                End If
            End If
        Else
            bln_MigrationLocationUseOther = False
        End If

    End Sub

    Private Sub rbnAdvancedSettingsQuestion2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbnAdvancedSettingsQuestion2B.CheckedChanged, rbnAdvancedSettingsQuestion2A.CheckedChanged

        If rbnAdvancedSettingsQuestion2B.Checked Then
            If Not bln_MigrationEncryptionCustom Then
                bln_MigrationEncryptionCustom = True
                formCustomEncryption.ShowDialog()
            End If
        Else
            bln_MigrationEncryptionCustom = False
        End If

    End Sub

End Class