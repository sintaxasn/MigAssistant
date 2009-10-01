<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class formCustomEncryption
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(formCustomEncryption))
        Me.lblCustomEncryptionKey1 = New System.Windows.Forms.Label
        Me.tbxCustomEncryptionKey1 = New System.Windows.Forms.TextBox
        Me.btnCustomEncryptionClose = New System.Windows.Forms.Button
        Me.tbxCustomEncryptionKey2 = New System.Windows.Forms.TextBox
        Me.lblCustomEncryptionKey2 = New System.Windows.Forms.Label
        Me.lblCustomEncryptionBody = New System.Windows.Forms.Label
        Me.lblCustomEncryptionFooter = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lblCustomEncryptionKey1
        '
        Me.lblCustomEncryptionKey1.AutoSize = True
        Me.lblCustomEncryptionKey1.Location = New System.Drawing.Point(12, 45)
        Me.lblCustomEncryptionKey1.Name = "lblCustomEncryptionKey1"
        Me.lblCustomEncryptionKey1.Size = New System.Drawing.Size(69, 13)
        Me.lblCustomEncryptionKey1.TabIndex = 38
        Me.lblCustomEncryptionKey1.Text = "Custom Key:"
        '
        'tbxCustomEncryptionKey1
        '
        Me.tbxCustomEncryptionKey1.Location = New System.Drawing.Point(138, 42)
        Me.tbxCustomEncryptionKey1.Name = "tbxCustomEncryptionKey1"
        Me.tbxCustomEncryptionKey1.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbxCustomEncryptionKey1.Size = New System.Drawing.Size(246, 22)
        Me.tbxCustomEncryptionKey1.TabIndex = 0
        '
        'btnCustomEncryptionClose
        '
        Me.btnCustomEncryptionClose.Location = New System.Drawing.Point(269, 153)
        Me.btnCustomEncryptionClose.Name = "btnCustomEncryptionClose"
        Me.btnCustomEncryptionClose.Size = New System.Drawing.Size(116, 29)
        Me.btnCustomEncryptionClose.TabIndex = 2
        Me.btnCustomEncryptionClose.Text = "&Save && Close"
        Me.btnCustomEncryptionClose.UseVisualStyleBackColor = True
        '
        'tbxCustomEncryptionKey2
        '
        Me.tbxCustomEncryptionKey2.Location = New System.Drawing.Point(138, 76)
        Me.tbxCustomEncryptionKey2.Name = "tbxCustomEncryptionKey2"
        Me.tbxCustomEncryptionKey2.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.tbxCustomEncryptionKey2.Size = New System.Drawing.Size(246, 22)
        Me.tbxCustomEncryptionKey2.TabIndex = 1
        '
        'lblCustomEncryptionKey2
        '
        Me.lblCustomEncryptionKey2.AutoSize = True
        Me.lblCustomEncryptionKey2.Location = New System.Drawing.Point(12, 79)
        Me.lblCustomEncryptionKey2.Name = "lblCustomEncryptionKey2"
        Me.lblCustomEncryptionKey2.Size = New System.Drawing.Size(116, 13)
        Me.lblCustomEncryptionKey2.TabIndex = 42
        Me.lblCustomEncryptionKey2.Text = "Re-Enter Custom Key:"
        '
        'lblCustomEncryptionBody
        '
        Me.lblCustomEncryptionBody.Location = New System.Drawing.Point(12, 9)
        Me.lblCustomEncryptionBody.Name = "lblCustomEncryptionBody"
        Me.lblCustomEncryptionBody.Size = New System.Drawing.Size(500, 16)
        Me.lblCustomEncryptionBody.TabIndex = 43
        Me.lblCustomEncryptionBody.Text = "Please specify your custom encryption key in the boxes provided below." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'lblCustomEncryptionFooter
        '
        Me.lblCustomEncryptionFooter.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblCustomEncryptionFooter.ForeColor = System.Drawing.Color.Red
        Me.lblCustomEncryptionFooter.Location = New System.Drawing.Point(11, 113)
        Me.lblCustomEncryptionFooter.Name = "lblCustomEncryptionFooter"
        Me.lblCustomEncryptionFooter.Size = New System.Drawing.Size(373, 37)
        Me.lblCustomEncryptionFooter.TabIndex = 44
        Me.lblCustomEncryptionFooter.Text = "If you forget or lose your key, your IT administrator cannot retreive your data!"
        Me.lblCustomEncryptionFooter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'formCustomEncryption
        '
        Me.AcceptButton = Me.btnCustomEncryptionClose
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(396, 194)
        Me.ControlBox = False
        Me.Controls.Add(Me.lblCustomEncryptionFooter)
        Me.Controls.Add(Me.lblCustomEncryptionBody)
        Me.Controls.Add(Me.tbxCustomEncryptionKey2)
        Me.Controls.Add(Me.lblCustomEncryptionKey2)
        Me.Controls.Add(Me.btnCustomEncryptionClose)
        Me.Controls.Add(Me.tbxCustomEncryptionKey1)
        Me.Controls.Add(Me.lblCustomEncryptionKey1)
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "formCustomEncryption"
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Custom Encryption"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblCustomEncryptionKey1 As System.Windows.Forms.Label
    Friend WithEvents tbxCustomEncryptionKey1 As System.Windows.Forms.TextBox
    Friend WithEvents btnCustomEncryptionClose As System.Windows.Forms.Button
    Friend WithEvents tbxCustomEncryptionKey2 As System.Windows.Forms.TextBox
    Friend WithEvents lblCustomEncryptionKey2 As System.Windows.Forms.Label
    Friend WithEvents lblCustomEncryptionBody As System.Windows.Forms.Label
    Friend WithEvents lblCustomEncryptionFooter As System.Windows.Forms.Label
End Class
