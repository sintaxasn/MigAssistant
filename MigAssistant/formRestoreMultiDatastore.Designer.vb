<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formRestoreMultiDatastore
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formRestoreMultiDatastore))
        Me.cbxRestoreMultiDatastoreList = New System.Windows.Forms.ComboBox
        Me.lblRestoreMultiDatastoreBody = New System.Windows.Forms.Label
        Me.btnRestoreMultiDatastoreContinue = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'cbxRestoreMultiDatastoreList
        '
        Me.cbxRestoreMultiDatastoreList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbxRestoreMultiDatastoreList.FormattingEnabled = True
        Me.cbxRestoreMultiDatastoreList.Location = New System.Drawing.Point(15, 64)
        Me.cbxRestoreMultiDatastoreList.Name = "cbxRestoreMultiDatastoreList"
        Me.cbxRestoreMultiDatastoreList.Size = New System.Drawing.Size(240, 21)
        Me.cbxRestoreMultiDatastoreList.Sorted = True
        Me.cbxRestoreMultiDatastoreList.TabIndex = 0
        '
        'lblRestoreMultiDatastoreBody
        '
        Me.lblRestoreMultiDatastoreBody.AutoSize = True
        Me.lblRestoreMultiDatastoreBody.Location = New System.Drawing.Point(12, 9)
        Me.lblRestoreMultiDatastoreBody.Name = "lblRestoreMultiDatastoreBody"
        Me.lblRestoreMultiDatastoreBody.Size = New System.Drawing.Size(399, 39)
        Me.lblRestoreMultiDatastoreBody.TabIndex = 1
        Me.lblRestoreMultiDatastoreBody.Text = "Multiple datastores have been detected for the current user." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Please select the" & _
            " datastore you want to restore from using the drop-down list below:"
        '
        'btnRestoreMultiDatastoreContinue
        '
        Me.btnRestoreMultiDatastoreContinue.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnRestoreMultiDatastoreContinue.Location = New System.Drawing.Point(295, 60)
        Me.btnRestoreMultiDatastoreContinue.Name = "btnRestoreMultiDatastoreContinue"
        Me.btnRestoreMultiDatastoreContinue.Size = New System.Drawing.Size(116, 27)
        Me.btnRestoreMultiDatastoreContinue.TabIndex = 2
        Me.btnRestoreMultiDatastoreContinue.Text = "&Continue"
        Me.btnRestoreMultiDatastoreContinue.UseVisualStyleBackColor = True
        '
        'formRestoreMultiDatastore
        '
        Me.AcceptButton = Me.btnRestoreMultiDatastoreContinue
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(427, 101)
        Me.ControlBox = False
        Me.Controls.Add(Me.btnRestoreMultiDatastoreContinue)
        Me.Controls.Add(Me.lblRestoreMultiDatastoreBody)
        Me.Controls.Add(Me.cbxRestoreMultiDatastoreList)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "formRestoreMultiDatastore"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Multiple Datastores Detected"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cbxRestoreMultiDatastoreList As System.Windows.Forms.ComboBox
    Friend WithEvents lblRestoreMultiDatastoreBody As System.Windows.Forms.Label
    Friend WithEvents btnRestoreMultiDatastoreContinue As System.Windows.Forms.Button
End Class
