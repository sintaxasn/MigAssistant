<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class form_Migration
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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(form_Migration))
        Me.label_OSVersion = New System.Windows.Forms.Label
        Me.button_Start = New System.Windows.Forms.Button
        Me.tooltips_MigrationForm = New System.Windows.Forms.ToolTip(Me.components)
        Me.button_AdvancedSettings = New System.Windows.Forms.Button
        Me.image_Header = New System.Windows.Forms.PictureBox
        Me.tabpage_Restore = New System.Windows.Forms.TabPage
        Me.slabel_DatastoreLocation = New System.Windows.Forms.Label
        Me.label_DatastoreLocation = New System.Windows.Forms.Label
        Me.tabpage_Capture = New System.Windows.Forms.TabPage
        Me.radiobox_WorkstationDetails1 = New System.Windows.Forms.RadioButton
        Me.radiobox_WorkstationDetails2 = New System.Windows.Forms.RadioButton
        Me.slabel_HealthCheck = New System.Windows.Forms.Label
        Me.checkbox_HealthCheck = New System.Windows.Forms.CheckBox
        Me.slabel_WorkstationDetails = New System.Windows.Forms.Label
        Me.tabcontrol_MigrationType = New System.Windows.Forms.TabControl
        Me.group_Status = New System.Windows.Forms.GroupBox
        Me.slabel_MigrationCurrentStatus = New System.Windows.Forms.Label
        Me.label_MigrationEstSize = New System.Windows.Forms.Label
        Me.slabel_MigrationEstTimeRemaining = New System.Windows.Forms.Label
        Me.slabel_MigrationEstSize = New System.Windows.Forms.Label
        Me.label_MigrationCurrentPhase = New System.Windows.Forms.Label
        Me.label_MigrationEstTimeRemaining = New System.Windows.Forms.Label
        Me.progressbar_Migration = New System.Windows.Forms.ProgressBar
        CType(Me.image_Header, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabpage_Restore.SuspendLayout()
        Me.tabpage_Capture.SuspendLayout()
        Me.tabcontrol_MigrationType.SuspendLayout()
        Me.group_Status.SuspendLayout()
        Me.SuspendLayout()
        '
        'label_OSVersion
        '
        Me.label_OSVersion.Font = New System.Drawing.Font("Segoe UI", 7.25!)
        Me.label_OSVersion.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.label_OSVersion.Location = New System.Drawing.Point(21, 320)
        Me.label_OSVersion.Name = "label_OSVersion"
        Me.label_OSVersion.Size = New System.Drawing.Size(308, 12)
        Me.label_OSVersion.TabIndex = 16
        Me.label_OSVersion.Text = "OS Version"
        Me.label_OSVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'button_Start
        '
        Me.button_Start.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.button_Start.Location = New System.Drawing.Point(466, 312)
        Me.button_Start.Name = "button_Start"
        Me.button_Start.Size = New System.Drawing.Size(116, 27)
        Me.button_Start.TabIndex = 0
        Me.button_Start.Text = "&Start"
        Me.tooltips_MigrationForm.SetToolTip(Me.button_Start, "Start / Stop the migration process")
        Me.button_Start.UseVisualStyleBackColor = True
        '
        'tooltips_MigrationForm
        '
        Me.tooltips_MigrationForm.AutoPopDelay = 5000
        Me.tooltips_MigrationForm.BackColor = System.Drawing.Color.AliceBlue
        Me.tooltips_MigrationForm.InitialDelay = 0
        Me.tooltips_MigrationForm.IsBalloon = True
        Me.tooltips_MigrationForm.ReshowDelay = 100
        Me.tooltips_MigrationForm.ToolTipTitle = "More Information"
        '
        'button_AdvancedSettings
        '
        Me.button_AdvancedSettings.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.button_AdvancedSettings.Location = New System.Drawing.Point(344, 312)
        Me.button_AdvancedSettings.Name = "button_AdvancedSettings"
        Me.button_AdvancedSettings.Size = New System.Drawing.Size(116, 27)
        Me.button_AdvancedSettings.TabIndex = 33
        Me.button_AdvancedSettings.Text = "&Advanced Settings"
        Me.tooltips_MigrationForm.SetToolTip(Me.button_AdvancedSettings, "Opens the advanced settings dialog, which allows you to further configure the mig" & _
                "ration process")
        Me.button_AdvancedSettings.UseVisualStyleBackColor = True
        Me.button_AdvancedSettings.Visible = False
        '
        'image_Header
        '
        Me.image_Header.Dock = System.Windows.Forms.DockStyle.Top
        Me.image_Header.Image = Global.MigAssistant.My.Resources.Resources.imageTitle
        Me.image_Header.Location = New System.Drawing.Point(0, 0)
        Me.image_Header.Name = "image_Header"
        Me.image_Header.Size = New System.Drawing.Size(591, 31)
        Me.image_Header.TabIndex = 0
        Me.image_Header.TabStop = False
        '
        'tabpage_Restore
        '
        Me.tabpage_Restore.Controls.Add(Me.slabel_DatastoreLocation)
        Me.tabpage_Restore.Controls.Add(Me.label_DatastoreLocation)
        Me.tabpage_Restore.Location = New System.Drawing.Point(4, 24)
        Me.tabpage_Restore.Name = "tabpage_Restore"
        Me.tabpage_Restore.Size = New System.Drawing.Size(566, 163)
        Me.tabpage_Restore.TabIndex = 5
        Me.tabpage_Restore.Text = "Data Restore"
        Me.tabpage_Restore.UseVisualStyleBackColor = True
        '
        'slabel_DatastoreLocation
        '
        Me.slabel_DatastoreLocation.AutoSize = True
        Me.slabel_DatastoreLocation.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.slabel_DatastoreLocation.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.slabel_DatastoreLocation.Location = New System.Drawing.Point(3, 12)
        Me.slabel_DatastoreLocation.Name = "slabel_DatastoreLocation"
        Me.slabel_DatastoreLocation.Size = New System.Drawing.Size(125, 19)
        Me.slabel_DatastoreLocation.TabIndex = 45
        Me.slabel_DatastoreLocation.Text = "Datastore Location"
        '
        'label_DatastoreLocation
        '
        Me.label_DatastoreLocation.AutoSize = True
        Me.label_DatastoreLocation.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.label_DatastoreLocation.ForeColor = System.Drawing.SystemColors.ControlText
        Me.label_DatastoreLocation.Location = New System.Drawing.Point(10, 31)
        Me.label_DatastoreLocation.Name = "label_DatastoreLocation"
        Me.label_DatastoreLocation.Size = New System.Drawing.Size(57, 13)
        Me.label_DatastoreLocation.TabIndex = 43
        Me.label_DatastoreLocation.Text = "Datastore"
        '
        'tabpage_Capture
        '
        Me.tabpage_Capture.Controls.Add(Me.radiobox_WorkstationDetails1)
        Me.tabpage_Capture.Controls.Add(Me.radiobox_WorkstationDetails2)
        Me.tabpage_Capture.Controls.Add(Me.slabel_HealthCheck)
        Me.tabpage_Capture.Controls.Add(Me.checkbox_HealthCheck)
        Me.tabpage_Capture.Controls.Add(Me.slabel_WorkstationDetails)
        Me.tabpage_Capture.Location = New System.Drawing.Point(4, 24)
        Me.tabpage_Capture.Name = "tabpage_Capture"
        Me.tabpage_Capture.Size = New System.Drawing.Size(566, 133)
        Me.tabpage_Capture.TabIndex = 2
        Me.tabpage_Capture.Text = "Data Capture"
        Me.tabpage_Capture.UseVisualStyleBackColor = True
        '
        'radiobox_WorkstationDetails1
        '
        Me.radiobox_WorkstationDetails1.AutoSize = True
        Me.radiobox_WorkstationDetails1.Checked = True
        Me.radiobox_WorkstationDetails1.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.radiobox_WorkstationDetails1.Location = New System.Drawing.Point(13, 34)
        Me.radiobox_WorkstationDetails1.Name = "radiobox_WorkstationDetails1"
        Me.radiobox_WorkstationDetails1.Size = New System.Drawing.Size(460, 17)
        Me.radiobox_WorkstationDetails1.TabIndex = 38
        Me.radiobox_WorkstationDetails1.TabStop = True
        Me.radiobox_WorkstationDetails1.Text = "&I am the only person who uses this Workstation. Only my data needs to be migrate" & _
            "d."
        Me.radiobox_WorkstationDetails1.UseVisualStyleBackColor = True
        '
        'radiobox_WorkstationDetails2
        '
        Me.radiobox_WorkstationDetails2.AutoSize = True
        Me.radiobox_WorkstationDetails2.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.radiobox_WorkstationDetails2.Location = New System.Drawing.Point(13, 58)
        Me.radiobox_WorkstationDetails2.Name = "radiobox_WorkstationDetails2"
        Me.radiobox_WorkstationDetails2.Size = New System.Drawing.Size(372, 17)
        Me.radiobox_WorkstationDetails2.TabIndex = 39
        Me.radiobox_WorkstationDetails2.Text = "&This is a shared Workstation. Everyone's data needs to be migrated."
        Me.radiobox_WorkstationDetails2.UseVisualStyleBackColor = True
        '
        'slabel_HealthCheck
        '
        Me.slabel_HealthCheck.AutoSize = True
        Me.slabel_HealthCheck.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.slabel_HealthCheck.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.slabel_HealthCheck.Location = New System.Drawing.Point(3, 85)
        Me.slabel_HealthCheck.Name = "slabel_HealthCheck"
        Me.slabel_HealthCheck.Size = New System.Drawing.Size(90, 19)
        Me.slabel_HealthCheck.TabIndex = 37
        Me.slabel_HealthCheck.Text = "Health Check"
        '
        'checkbox_HealthCheck
        '
        Me.checkbox_HealthCheck.Font = New System.Drawing.Font("Segoe UI", 8.0!)
        Me.checkbox_HealthCheck.Location = New System.Drawing.Point(13, 107)
        Me.checkbox_HealthCheck.Name = "checkbox_HealthCheck"
        Me.checkbox_HealthCheck.Size = New System.Drawing.Size(543, 22)
        Me.checkbox_HealthCheck.TabIndex = 35
        Me.checkbox_HealthCheck.Text = "Automatically examine my &Hard Disk for any errors that could potentially cause t" & _
            "he capture to fail."
        Me.checkbox_HealthCheck.UseVisualStyleBackColor = False
        '
        'slabel_WorkstationDetails
        '
        Me.slabel_WorkstationDetails.AutoSize = True
        Me.slabel_WorkstationDetails.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.slabel_WorkstationDetails.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.slabel_WorkstationDetails.Location = New System.Drawing.Point(3, 12)
        Me.slabel_WorkstationDetails.Name = "slabel_WorkstationDetails"
        Me.slabel_WorkstationDetails.Size = New System.Drawing.Size(129, 19)
        Me.slabel_WorkstationDetails.TabIndex = 24
        Me.slabel_WorkstationDetails.Text = "Workstation Details"
        '
        'tabcontrol_MigrationType
        '
        Me.tabcontrol_MigrationType.Controls.Add(Me.tabpage_Capture)
        Me.tabcontrol_MigrationType.Controls.Add(Me.tabpage_Restore)
        Me.tabcontrol_MigrationType.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.tabcontrol_MigrationType.Location = New System.Drawing.Point(12, 37)
        Me.tabcontrol_MigrationType.Multiline = True
        Me.tabcontrol_MigrationType.Name = "tabcontrol_MigrationType"
        Me.tabcontrol_MigrationType.Padding = New System.Drawing.Point(10, 3)
        Me.tabcontrol_MigrationType.SelectedIndex = 0
        Me.tabcontrol_MigrationType.Size = New System.Drawing.Size(574, 161)
        Me.tabcontrol_MigrationType.TabIndex = 6
        Me.tabcontrol_MigrationType.TabStop = False
        '
        'group_Status
        '
        Me.group_Status.Controls.Add(Me.slabel_MigrationCurrentStatus)
        Me.group_Status.Controls.Add(Me.label_MigrationEstSize)
        Me.group_Status.Controls.Add(Me.slabel_MigrationEstTimeRemaining)
        Me.group_Status.Controls.Add(Me.slabel_MigrationEstSize)
        Me.group_Status.Controls.Add(Me.label_MigrationCurrentPhase)
        Me.group_Status.Controls.Add(Me.label_MigrationEstTimeRemaining)
        Me.group_Status.Enabled = False
        Me.group_Status.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.group_Status.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.group_Status.Location = New System.Drawing.Point(12, 204)
        Me.group_Status.Name = "group_Status"
        Me.group_Status.Size = New System.Drawing.Size(570, 102)
        Me.group_Status.TabIndex = 57
        Me.group_Status.TabStop = False
        Me.group_Status.Text = "Status"
        '
        'slabel_MigrationCurrentStatus
        '
        Me.slabel_MigrationCurrentStatus.AutoSize = True
        Me.slabel_MigrationCurrentStatus.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.slabel_MigrationCurrentStatus.ForeColor = System.Drawing.Color.Black
        Me.slabel_MigrationCurrentStatus.Location = New System.Drawing.Point(14, 21)
        Me.slabel_MigrationCurrentStatus.Name = "slabel_MigrationCurrentStatus"
        Me.slabel_MigrationCurrentStatus.Size = New System.Drawing.Size(84, 13)
        Me.slabel_MigrationCurrentStatus.TabIndex = 55
        Me.slabel_MigrationCurrentStatus.Text = "Current Status:"
        '
        'label_MigrationEstSize
        '
        Me.label_MigrationEstSize.AutoSize = True
        Me.label_MigrationEstSize.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.label_MigrationEstSize.ForeColor = System.Drawing.Color.Black
        Me.label_MigrationEstSize.Location = New System.Drawing.Point(175, 57)
        Me.label_MigrationEstSize.Name = "label_MigrationEstSize"
        Me.label_MigrationEstSize.Size = New System.Drawing.Size(58, 13)
        Me.label_MigrationEstSize.TabIndex = 54
        Me.label_MigrationEstSize.Text = "Unknown"
        '
        'slabel_MigrationEstTimeRemaining
        '
        Me.slabel_MigrationEstTimeRemaining.AutoSize = True
        Me.slabel_MigrationEstTimeRemaining.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.slabel_MigrationEstTimeRemaining.ForeColor = System.Drawing.Color.Black
        Me.slabel_MigrationEstTimeRemaining.Location = New System.Drawing.Point(14, 80)
        Me.slabel_MigrationEstTimeRemaining.Name = "slabel_MigrationEstTimeRemaining"
        Me.slabel_MigrationEstTimeRemaining.Size = New System.Drawing.Size(144, 13)
        Me.slabel_MigrationEstTimeRemaining.TabIndex = 53
        Me.slabel_MigrationEstTimeRemaining.Text = "Estimated Time Remaining:"
        '
        'slabel_MigrationEstSize
        '
        Me.slabel_MigrationEstSize.AutoSize = True
        Me.slabel_MigrationEstSize.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.slabel_MigrationEstSize.ForeColor = System.Drawing.Color.Black
        Me.slabel_MigrationEstSize.Location = New System.Drawing.Point(14, 57)
        Me.slabel_MigrationEstSize.Name = "slabel_MigrationEstSize"
        Me.slabel_MigrationEstSize.Size = New System.Drawing.Size(137, 13)
        Me.slabel_MigrationEstSize.TabIndex = 52
        Me.slabel_MigrationEstSize.Text = "Estimated Migration Size:"
        '
        'label_MigrationCurrentPhase
        '
        Me.label_MigrationCurrentPhase.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.label_MigrationCurrentPhase.ForeColor = System.Drawing.Color.Black
        Me.label_MigrationCurrentPhase.Location = New System.Drawing.Point(175, 21)
        Me.label_MigrationCurrentPhase.Name = "label_MigrationCurrentPhase"
        Me.label_MigrationCurrentPhase.Size = New System.Drawing.Size(385, 36)
        Me.label_MigrationCurrentPhase.TabIndex = 50
        Me.label_MigrationCurrentPhase.Text = "Unknown"
        '
        'label_MigrationEstTimeRemaining
        '
        Me.label_MigrationEstTimeRemaining.AutoSize = True
        Me.label_MigrationEstTimeRemaining.Font = New System.Drawing.Font("Segoe UI", 8.25!)
        Me.label_MigrationEstTimeRemaining.ForeColor = System.Drawing.Color.Black
        Me.label_MigrationEstTimeRemaining.Location = New System.Drawing.Point(175, 80)
        Me.label_MigrationEstTimeRemaining.Name = "label_MigrationEstTimeRemaining"
        Me.label_MigrationEstTimeRemaining.Size = New System.Drawing.Size(58, 13)
        Me.label_MigrationEstTimeRemaining.TabIndex = 51
        Me.label_MigrationEstTimeRemaining.Text = "Unknown"
        '
        'progressbar_Migration
        '
        Me.progressbar_Migration.Location = New System.Drawing.Point(12, 314)
        Me.progressbar_Migration.Name = "progressbar_Migration"
        Me.progressbar_Migration.Size = New System.Drawing.Size(326, 22)
        Me.progressbar_Migration.Step = 1
        Me.progressbar_Migration.TabIndex = 52
        Me.progressbar_Migration.Visible = False
        '
        'form_Migration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(591, 347)
        Me.Controls.Add(Me.progressbar_Migration)
        Me.Controls.Add(Me.tabcontrol_MigrationType)
        Me.Controls.Add(Me.group_Status)
        Me.Controls.Add(Me.button_AdvancedSettings)
        Me.Controls.Add(Me.image_Header)
        Me.Controls.Add(Me.button_Start)
        Me.Controls.Add(Me.label_OSVersion)
        Me.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "form_Migration"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Workstation Migration Assistant"
        CType(Me.image_Header, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabpage_Restore.ResumeLayout(False)
        Me.tabpage_Restore.PerformLayout()
        Me.tabpage_Capture.ResumeLayout(False)
        Me.tabpage_Capture.PerformLayout()
        Me.tabcontrol_MigrationType.ResumeLayout(False)
        Me.group_Status.ResumeLayout(False)
        Me.group_Status.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents image_Header As System.Windows.Forms.PictureBox
    Friend WithEvents tooltips_MigrationForm As System.Windows.Forms.ToolTip
    Friend WithEvents button_Start As System.Windows.Forms.Button
    Friend WithEvents label_OSVersion As System.Windows.Forms.Label
    Friend WithEvents tabpage_Restore As System.Windows.Forms.TabPage
    Friend WithEvents label_DatastoreLocation As System.Windows.Forms.Label
    Friend WithEvents tabpage_Capture As System.Windows.Forms.TabPage
    Friend WithEvents checkbox_HealthCheck As System.Windows.Forms.CheckBox
    Friend WithEvents button_AdvancedSettings As System.Windows.Forms.Button
    Friend WithEvents slabel_WorkstationDetails As System.Windows.Forms.Label
    Friend WithEvents tabcontrol_MigrationType As System.Windows.Forms.TabControl
    Friend WithEvents group_Status As System.Windows.Forms.GroupBox
    Friend WithEvents progressbar_Migration As System.Windows.Forms.ProgressBar
    Friend WithEvents label_MigrationEstTimeRemaining As System.Windows.Forms.Label
    Friend WithEvents slabel_MigrationEstTimeRemaining As System.Windows.Forms.Label
    Friend WithEvents slabel_MigrationEstSize As System.Windows.Forms.Label
    Friend WithEvents label_MigrationEstSize As System.Windows.Forms.Label
    Friend WithEvents radiobox_WorkstationDetails1 As System.Windows.Forms.RadioButton
    Friend WithEvents radiobox_WorkstationDetails2 As System.Windows.Forms.RadioButton
    Friend WithEvents slabel_HealthCheck As System.Windows.Forms.Label
    Friend WithEvents slabel_MigrationCurrentStatus As System.Windows.Forms.Label
    Friend WithEvents label_MigrationCurrentPhase As System.Windows.Forms.Label
    Friend WithEvents slabel_DatastoreLocation As System.Windows.Forms.Label

End Class
