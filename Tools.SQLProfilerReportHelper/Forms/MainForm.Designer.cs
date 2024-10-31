namespace Tools.SQLProfilerReportHelper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBoxConnect = new System.Windows.Forms.GroupBox();
            this._connectedLabel = new System.Windows.Forms.Label();
            this._dbTextBox = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this._serverTextBox = new System.Windows.Forms.TextBox();
            this.labelDB = new System.Windows.Forms.Label();
            this.labelSQLServer = new System.Windows.Forms.Label();
            this._groupBoxTable = new System.Windows.Forms.GroupBox();
            this._comboBoxTable = new System.Windows.Forms.ComboBox();
            this.labelTable = new System.Windows.Forms.Label();
            this._groupBoxNormalization = new System.Windows.Forms.GroupBox();
            this.labelTextKeyStatus = new System.Windows.Forms.Label();
            this._textBoxStartTime = new System.Windows.Forms.TextBox();
            this._textBoxExpectedEndTime = new System.Windows.Forms.TextBox();
            this.textBoxPreparedRowProgress = new System.Windows.Forms.TextBox();
            this._textBoxPreparedRowCount = new System.Windows.Forms.TextBox();
            this._textBoxRowCount = new System.Windows.Forms.TextBox();
            this.labelPreparedRowProgress = new System.Windows.Forms.Label();
            this.labelStopTime = new System.Windows.Forms.Label();
            this.labelPreparedRowCount = new System.Windows.Forms.Label();
            this.labelStartTime = new System.Windows.Forms.Label();
            this._buttonStartNormalization = new System.Windows.Forms.Button();
            this.labelRowCount = new System.Windows.Forms.Label();
            this._groupBoxReports = new System.Windows.Forms.GroupBox();
            this.buttonDeadlockReportCreate = new System.Windows.Forms.Button();
            this.buttonDetailReportView = new System.Windows.Forms.Button();
            this._checkBoxDeadlockReportStatus = new System.Windows.Forms.CheckBox();
            this._buttonDetailReportCreate = new System.Windows.Forms.Button();
            this.labelDeadlockReportStatus = new System.Windows.Forms.Label();
            this._buttonErrorReportCreate = new System.Windows.Forms.Button();
            this._checkBoxDetailReportStatus = new System.Windows.Forms.CheckBox();
            this.labelDetailReportStatus = new System.Windows.Forms.Label();
            this._checkBoxErrorReportStatus = new System.Windows.Forms.CheckBox();
            this.labelErrorReportStatus = new System.Windows.Forms.Label();
            this.panelConsole = new System.Windows.Forms.Panel();
            this._panelRight = new System.Windows.Forms.Panel();
            this._groupBoxImportFile = new System.Windows.Forms.GroupBox();
            this._forceOverrideCheckBox = new System.Windows.Forms.CheckBox();
            this._trcFilePathTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._importTableComboBox = new System.Windows.Forms.ComboBox();
            this._buttonImport = new System.Windows.Forms.Button();
            this._groupBoxTrace = new System.Windows.Forms.GroupBox();
            this._buttonStartNewTrace = new System.Windows.Forms.Button();
            this._panelLeft = new System.Windows.Forms.Panel();
            this.groupBoxConnect.SuspendLayout();
            this._groupBoxTable.SuspendLayout();
            this._groupBoxNormalization.SuspendLayout();
            this._groupBoxReports.SuspendLayout();
            this.panelConsole.SuspendLayout();
            this._panelRight.SuspendLayout();
            this._groupBoxImportFile.SuspendLayout();
            this._groupBoxTrace.SuspendLayout();
            this._panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConnect
            // 
            this.groupBoxConnect.Controls.Add(this._connectedLabel);
            this.groupBoxConnect.Controls.Add(this._dbTextBox);
            this.groupBoxConnect.Controls.Add(this.buttonConnect);
            this.groupBoxConnect.Controls.Add(this._serverTextBox);
            this.groupBoxConnect.Controls.Add(this.labelDB);
            this.groupBoxConnect.Controls.Add(this.labelSQLServer);
            this.groupBoxConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxConnect.Location = new System.Drawing.Point(0, 0);
            this.groupBoxConnect.Name = "groupBoxConnect";
            this.groupBoxConnect.Size = new System.Drawing.Size(509, 77);
            this.groupBoxConnect.TabIndex = 1;
            this.groupBoxConnect.TabStop = false;
            this.groupBoxConnect.Text = "Connect";
            // 
            // _connectedLabel
            // 
            this._connectedLabel.AutoSize = true;
            this._connectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._connectedLabel.ForeColor = System.Drawing.Color.Green;
            this._connectedLabel.Location = new System.Drawing.Point(401, 47);
            this._connectedLabel.Name = "_connectedLabel";
            this._connectedLabel.Size = new System.Drawing.Size(79, 16);
            this._connectedLabel.TabIndex = 39;
            this._connectedLabel.Text = "connected";
            this._connectedLabel.Visible = false;
            // 
            // _dbTextBox
            // 
            this._dbTextBox.Location = new System.Drawing.Point(133, 45);
            this._dbTextBox.Name = "_dbTextBox";
            this._dbTextBox.ReadOnly = true;
            this._dbTextBox.Size = new System.Drawing.Size(251, 20);
            this._dbTextBox.TabIndex = 38;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonConnect.Location = new System.Drawing.Point(390, 17);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(100, 23);
            this.buttonConnect.TabIndex = 27;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // _serverTextBox
            // 
            this._serverTextBox.Location = new System.Drawing.Point(133, 19);
            this._serverTextBox.Name = "_serverTextBox";
            this._serverTextBox.ReadOnly = true;
            this._serverTextBox.Size = new System.Drawing.Size(251, 20);
            this._serverTextBox.TabIndex = 37;
            // 
            // labelDB
            // 
            this.labelDB.AutoSize = true;
            this.labelDB.Location = new System.Drawing.Point(7, 49);
            this.labelDB.Name = "labelDB";
            this.labelDB.Size = new System.Drawing.Size(56, 13);
            this.labelDB.TabIndex = 24;
            this.labelDB.Text = "Database:";
            // 
            // labelSQLServer
            // 
            this.labelSQLServer.AutoSize = true;
            this.labelSQLServer.Location = new System.Drawing.Point(7, 22);
            this.labelSQLServer.Name = "labelSQLServer";
            this.labelSQLServer.Size = new System.Drawing.Size(65, 13);
            this.labelSQLServer.TabIndex = 23;
            this.labelSQLServer.Text = "SQL Server:";
            // 
            // _groupBoxTable
            // 
            this._groupBoxTable.Controls.Add(this._comboBoxTable);
            this._groupBoxTable.Controls.Add(this.labelTable);
            this._groupBoxTable.Dock = System.Windows.Forms.DockStyle.Top;
            this._groupBoxTable.Location = new System.Drawing.Point(0, 77);
            this._groupBoxTable.Name = "_groupBoxTable";
            this._groupBoxTable.Size = new System.Drawing.Size(509, 45);
            this._groupBoxTable.TabIndex = 2;
            this._groupBoxTable.TabStop = false;
            this._groupBoxTable.Text = "Table";
            // 
            // _comboBoxTable
            // 
            this._comboBoxTable.FormattingEnabled = true;
            this._comboBoxTable.Location = new System.Drawing.Point(135, 16);
            this._comboBoxTable.Name = "_comboBoxTable";
            this._comboBoxTable.Size = new System.Drawing.Size(251, 21);
            this._comboBoxTable.TabIndex = 27;
            this._comboBoxTable.TextChanged += new System.EventHandler(this.ComboBoxTable_TextChanged);
            // 
            // labelTable
            // 
            this.labelTable.AutoSize = true;
            this.labelTable.Location = new System.Drawing.Point(9, 19);
            this.labelTable.Name = "labelTable";
            this.labelTable.Size = new System.Drawing.Size(77, 13);
            this.labelTable.TabIndex = 22;
            this.labelTable.Text = "Profiling Table:";
            // 
            // _groupBoxNormalization
            // 
            this._groupBoxNormalization.Controls.Add(this.labelTextKeyStatus);
            this._groupBoxNormalization.Controls.Add(this._textBoxStartTime);
            this._groupBoxNormalization.Controls.Add(this._textBoxExpectedEndTime);
            this._groupBoxNormalization.Controls.Add(this.textBoxPreparedRowProgress);
            this._groupBoxNormalization.Controls.Add(this._textBoxPreparedRowCount);
            this._groupBoxNormalization.Controls.Add(this._textBoxRowCount);
            this._groupBoxNormalization.Controls.Add(this.labelPreparedRowProgress);
            this._groupBoxNormalization.Controls.Add(this.labelStopTime);
            this._groupBoxNormalization.Controls.Add(this.labelPreparedRowCount);
            this._groupBoxNormalization.Controls.Add(this.labelStartTime);
            this._groupBoxNormalization.Controls.Add(this._buttonStartNormalization);
            this._groupBoxNormalization.Controls.Add(this.labelRowCount);
            this._groupBoxNormalization.Dock = System.Windows.Forms.DockStyle.Top;
            this._groupBoxNormalization.Location = new System.Drawing.Point(0, 122);
            this._groupBoxNormalization.Name = "_groupBoxNormalization";
            this._groupBoxNormalization.Size = new System.Drawing.Size(509, 182);
            this._groupBoxNormalization.TabIndex = 27;
            this._groupBoxNormalization.TabStop = false;
            this._groupBoxNormalization.Text = "Normalization";
            // 
            // labelTextKeyStatus
            // 
            this.labelTextKeyStatus.AutoSize = true;
            this.labelTextKeyStatus.Location = new System.Drawing.Point(15, 22);
            this.labelTextKeyStatus.Name = "labelTextKeyStatus";
            this.labelTextKeyStatus.Size = new System.Drawing.Size(103, 13);
            this.labelTextKeyStatus.TabIndex = 31;
            this.labelTextKeyStatus.Text = "Normalize TextData:";
            // 
            // _textBoxStartTime
            // 
            this._textBoxStartTime.Location = new System.Drawing.Point(135, 44);
            this._textBoxStartTime.Name = "_textBoxStartTime";
            this._textBoxStartTime.ReadOnly = true;
            this._textBoxStartTime.Size = new System.Drawing.Size(251, 20);
            this._textBoxStartTime.TabIndex = 27;
            // 
            // _textBoxExpectedEndTime
            // 
            this._textBoxExpectedEndTime.Location = new System.Drawing.Point(135, 148);
            this._textBoxExpectedEndTime.Name = "_textBoxExpectedEndTime";
            this._textBoxExpectedEndTime.ReadOnly = true;
            this._textBoxExpectedEndTime.Size = new System.Drawing.Size(251, 20);
            this._textBoxExpectedEndTime.TabIndex = 30;
            // 
            // textBoxPreparedRowProgress
            // 
            this.textBoxPreparedRowProgress.Location = new System.Drawing.Point(135, 122);
            this.textBoxPreparedRowProgress.Name = "textBoxPreparedRowProgress";
            this.textBoxPreparedRowProgress.ReadOnly = true;
            this.textBoxPreparedRowProgress.Size = new System.Drawing.Size(251, 20);
            this.textBoxPreparedRowProgress.TabIndex = 29;
            // 
            // _textBoxPreparedRowCount
            // 
            this._textBoxPreparedRowCount.Location = new System.Drawing.Point(135, 96);
            this._textBoxPreparedRowCount.Name = "_textBoxPreparedRowCount";
            this._textBoxPreparedRowCount.ReadOnly = true;
            this._textBoxPreparedRowCount.Size = new System.Drawing.Size(251, 20);
            this._textBoxPreparedRowCount.TabIndex = 28;
            // 
            // _textBoxRowCount
            // 
            this._textBoxRowCount.Location = new System.Drawing.Point(135, 70);
            this._textBoxRowCount.Name = "_textBoxRowCount";
            this._textBoxRowCount.ReadOnly = true;
            this._textBoxRowCount.Size = new System.Drawing.Size(251, 20);
            this._textBoxRowCount.TabIndex = 26;
            // 
            // labelPreparedRowProgress
            // 
            this.labelPreparedRowProgress.AutoSize = true;
            this.labelPreparedRowProgress.Location = new System.Drawing.Point(15, 125);
            this.labelPreparedRowProgress.Name = "labelPreparedRowProgress";
            this.labelPreparedRowProgress.Size = new System.Drawing.Size(116, 13);
            this.labelPreparedRowProgress.TabIndex = 25;
            this.labelPreparedRowProgress.Text = "Prepared row progress:";
            // 
            // labelStopTime
            // 
            this.labelStopTime.AutoSize = true;
            this.labelStopTime.Location = new System.Drawing.Point(15, 151);
            this.labelStopTime.Name = "labelStopTime";
            this.labelStopTime.Size = new System.Drawing.Size(54, 13);
            this.labelStopTime.TabIndex = 24;
            this.labelStopTime.Text = "Stop time:";
            // 
            // labelPreparedRowCount
            // 
            this.labelPreparedRowCount.AutoSize = true;
            this.labelPreparedRowCount.Location = new System.Drawing.Point(15, 99);
            this.labelPreparedRowCount.Name = "labelPreparedRowCount";
            this.labelPreparedRowCount.Size = new System.Drawing.Size(103, 13);
            this.labelPreparedRowCount.TabIndex = 23;
            this.labelPreparedRowCount.Text = "Prepared row count:";
            // 
            // labelStartTime
            // 
            this.labelStartTime.AutoSize = true;
            this.labelStartTime.Location = new System.Drawing.Point(15, 47);
            this.labelStartTime.Name = "labelStartTime";
            this.labelStartTime.Size = new System.Drawing.Size(54, 13);
            this.labelStartTime.TabIndex = 22;
            this.labelStartTime.Text = "Start time:";
            // 
            // _buttonStartNormalization
            // 
            this._buttonStartNormalization.Enabled = false;
            this._buttonStartNormalization.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._buttonStartNormalization.Location = new System.Drawing.Point(135, 17);
            this._buttonStartNormalization.Name = "_buttonStartNormalization";
            this._buttonStartNormalization.Size = new System.Drawing.Size(100, 23);
            this._buttonStartNormalization.TabIndex = 20;
            this._buttonStartNormalization.Text = "Normalize";
            this._buttonStartNormalization.UseVisualStyleBackColor = true;
            this._buttonStartNormalization.Click += new System.EventHandler(this.ButtonStartNormalization_Click);
            // 
            // labelRowCount
            // 
            this.labelRowCount.AutoSize = true;
            this.labelRowCount.Location = new System.Drawing.Point(15, 73);
            this.labelRowCount.Name = "labelRowCount";
            this.labelRowCount.Size = new System.Drawing.Size(62, 13);
            this.labelRowCount.TabIndex = 19;
            this.labelRowCount.Text = "Row count:";
            // 
            // _groupBoxReports
            // 
            this._groupBoxReports.Controls.Add(this.buttonDeadlockReportCreate);
            this._groupBoxReports.Controls.Add(this.buttonDetailReportView);
            this._groupBoxReports.Controls.Add(this._checkBoxDeadlockReportStatus);
            this._groupBoxReports.Controls.Add(this._buttonDetailReportCreate);
            this._groupBoxReports.Controls.Add(this.labelDeadlockReportStatus);
            this._groupBoxReports.Controls.Add(this._buttonErrorReportCreate);
            this._groupBoxReports.Controls.Add(this._checkBoxDetailReportStatus);
            this._groupBoxReports.Controls.Add(this.labelDetailReportStatus);
            this._groupBoxReports.Controls.Add(this._checkBoxErrorReportStatus);
            this._groupBoxReports.Controls.Add(this.labelErrorReportStatus);
            this._groupBoxReports.Dock = System.Windows.Forms.DockStyle.Top;
            this._groupBoxReports.Location = new System.Drawing.Point(0, 304);
            this._groupBoxReports.Name = "_groupBoxReports";
            this._groupBoxReports.Size = new System.Drawing.Size(509, 133);
            this._groupBoxReports.TabIndex = 4;
            this._groupBoxReports.TabStop = false;
            this._groupBoxReports.Text = "Reports";
            // 
            // buttonDeadlockReportCreate
            // 
            this.buttonDeadlockReportCreate.Enabled = false;
            this.buttonDeadlockReportCreate.Location = new System.Drawing.Point(233, 101);
            this.buttonDeadlockReportCreate.Name = "buttonDeadlockReportCreate";
            this.buttonDeadlockReportCreate.Size = new System.Drawing.Size(74, 23);
            this.buttonDeadlockReportCreate.TabIndex = 38;
            this.buttonDeadlockReportCreate.Text = "Create";
            this.buttonDeadlockReportCreate.UseVisualStyleBackColor = true;
            this.buttonDeadlockReportCreate.Click += new System.EventHandler(this.ButtonDeadlockReportCreate_Click);
            // 
            // buttonDetailReportView
            // 
            this.buttonDetailReportView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonDetailReportView.Location = new System.Drawing.Point(390, 17);
            this.buttonDetailReportView.Name = "buttonDetailReportView";
            this.buttonDetailReportView.Size = new System.Drawing.Size(100, 23);
            this.buttonDetailReportView.TabIndex = 31;
            this.buttonDetailReportView.Text = "View";
            this.buttonDetailReportView.UseVisualStyleBackColor = true;
            this.buttonDetailReportView.Click += new System.EventHandler(this.ButtonDetailReportView_Click);
            // 
            // _checkBoxDeadlockReportStatus
            // 
            this._checkBoxDeadlockReportStatus.AutoSize = true;
            this._checkBoxDeadlockReportStatus.Enabled = false;
            this._checkBoxDeadlockReportStatus.Location = new System.Drawing.Point(135, 105);
            this._checkBoxDeadlockReportStatus.Name = "_checkBoxDeadlockReportStatus";
            this._checkBoxDeadlockReportStatus.Size = new System.Drawing.Size(82, 17);
            this._checkBoxDeadlockReportStatus.TabIndex = 36;
            this._checkBoxDeadlockReportStatus.Text = "Report exist";
            this._checkBoxDeadlockReportStatus.UseVisualStyleBackColor = true;
            // 
            // _buttonDetailReportCreate
            // 
            this._buttonDetailReportCreate.Enabled = false;
            this._buttonDetailReportCreate.Location = new System.Drawing.Point(233, 19);
            this._buttonDetailReportCreate.Name = "_buttonDetailReportCreate";
            this._buttonDetailReportCreate.Size = new System.Drawing.Size(74, 23);
            this._buttonDetailReportCreate.TabIndex = 30;
            this._buttonDetailReportCreate.Text = "Create";
            this._buttonDetailReportCreate.UseVisualStyleBackColor = true;
            this._buttonDetailReportCreate.Click += new System.EventHandler(this.ButtonDetailReportCreate_Click);
            // 
            // labelDeadlockReportStatus
            // 
            this.labelDeadlockReportStatus.AutoSize = true;
            this.labelDeadlockReportStatus.Location = new System.Drawing.Point(7, 105);
            this.labelDeadlockReportStatus.Name = "labelDeadlockReportStatus";
            this.labelDeadlockReportStatus.Size = new System.Drawing.Size(117, 13);
            this.labelDeadlockReportStatus.TabIndex = 35;
            this.labelDeadlockReportStatus.Text = "Deadlock report status:";
            // 
            // _buttonErrorReportCreate
            // 
            this._buttonErrorReportCreate.Enabled = false;
            this._buttonErrorReportCreate.Location = new System.Drawing.Point(233, 66);
            this._buttonErrorReportCreate.Name = "_buttonErrorReportCreate";
            this._buttonErrorReportCreate.Size = new System.Drawing.Size(74, 23);
            this._buttonErrorReportCreate.TabIndex = 38;
            this._buttonErrorReportCreate.Text = "Create";
            this._buttonErrorReportCreate.UseVisualStyleBackColor = true;
            this._buttonErrorReportCreate.Click += new System.EventHandler(this.ButtonErrorReportCreate_Click);
            // 
            // _checkBoxDetailReportStatus
            // 
            this._checkBoxDetailReportStatus.AutoSize = true;
            this._checkBoxDetailReportStatus.Enabled = false;
            this._checkBoxDetailReportStatus.Location = new System.Drawing.Point(135, 23);
            this._checkBoxDetailReportStatus.Name = "_checkBoxDetailReportStatus";
            this._checkBoxDetailReportStatus.Size = new System.Drawing.Size(82, 17);
            this._checkBoxDetailReportStatus.TabIndex = 28;
            this._checkBoxDetailReportStatus.Text = "Report exist";
            this._checkBoxDetailReportStatus.UseVisualStyleBackColor = true;
            // 
            // labelDetailReportStatus
            // 
            this.labelDetailReportStatus.AutoSize = true;
            this.labelDetailReportStatus.Location = new System.Drawing.Point(7, 24);
            this.labelDetailReportStatus.Name = "labelDetailReportStatus";
            this.labelDetailReportStatus.Size = new System.Drawing.Size(98, 13);
            this.labelDetailReportStatus.TabIndex = 7;
            this.labelDetailReportStatus.Text = "Detail report status:";
            // 
            // _checkBoxErrorReportStatus
            // 
            this._checkBoxErrorReportStatus.AutoSize = true;
            this._checkBoxErrorReportStatus.Enabled = false;
            this._checkBoxErrorReportStatus.Location = new System.Drawing.Point(135, 70);
            this._checkBoxErrorReportStatus.Name = "_checkBoxErrorReportStatus";
            this._checkBoxErrorReportStatus.Size = new System.Drawing.Size(82, 17);
            this._checkBoxErrorReportStatus.TabIndex = 36;
            this._checkBoxErrorReportStatus.Text = "Report exist";
            this._checkBoxErrorReportStatus.UseVisualStyleBackColor = true;
            // 
            // labelErrorReportStatus
            // 
            this.labelErrorReportStatus.AutoSize = true;
            this.labelErrorReportStatus.Location = new System.Drawing.Point(6, 70);
            this.labelErrorReportStatus.Name = "labelErrorReportStatus";
            this.labelErrorReportStatus.Size = new System.Drawing.Size(93, 13);
            this.labelErrorReportStatus.TabIndex = 35;
            this.labelErrorReportStatus.Text = "Error report status:";
            // 
            // panelConsole
            // 
            this.panelConsole.Controls.Add(this._panelRight);
            this.panelConsole.Controls.Add(this._panelLeft);
            this.panelConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConsole.Location = new System.Drawing.Point(0, 0);
            this.panelConsole.Name = "panelConsole";
            this.panelConsole.Size = new System.Drawing.Size(898, 448);
            this.panelConsole.TabIndex = 2;
            // 
            // _panelRight
            // 
            this._panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panelRight.Controls.Add(this._groupBoxImportFile);
            this._panelRight.Controls.Add(this._groupBoxTrace);
            this._panelRight.Location = new System.Drawing.Point(518, 3);
            this._panelRight.Name = "_panelRight";
            this._panelRight.Size = new System.Drawing.Size(368, 437);
            this._panelRight.TabIndex = 44;
            // 
            // _groupBoxImportFile
            // 
            this._groupBoxImportFile.Controls.Add(this._forceOverrideCheckBox);
            this._groupBoxImportFile.Controls.Add(this._trcFilePathTextBox);
            this._groupBoxImportFile.Controls.Add(this.label3);
            this._groupBoxImportFile.Controls.Add(this.label1);
            this._groupBoxImportFile.Controls.Add(this._importTableComboBox);
            this._groupBoxImportFile.Controls.Add(this._buttonImport);
            this._groupBoxImportFile.Dock = System.Windows.Forms.DockStyle.Top;
            this._groupBoxImportFile.Location = new System.Drawing.Point(0, 304);
            this._groupBoxImportFile.Name = "_groupBoxImportFile";
            this._groupBoxImportFile.Size = new System.Drawing.Size(368, 133);
            this._groupBoxImportFile.TabIndex = 44;
            this._groupBoxImportFile.TabStop = false;
            this._groupBoxImportFile.Text = "Import file Into DB";
            // 
            // _forceOverrideCheckBox
            // 
            this._forceOverrideCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._forceOverrideCheckBox.AutoSize = true;
            this._forceOverrideCheckBox.Location = new System.Drawing.Point(111, 77);
            this._forceOverrideCheckBox.Name = "_forceOverrideCheckBox";
            this._forceOverrideCheckBox.Size = new System.Drawing.Size(161, 17);
            this._forceOverrideCheckBox.TabIndex = 34;
            this._forceOverrideCheckBox.Text = "Force owerride profiling table";
            this._forceOverrideCheckBox.UseVisualStyleBackColor = true;
            // 
            // _trcFilePathTextBox
            // 
            this._trcFilePathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._trcFilePathTextBox.Location = new System.Drawing.Point(111, 51);
            this._trcFilePathTextBox.Name = "_trcFilePathTextBox";
            this._trcFilePathTextBox.Size = new System.Drawing.Size(251, 20);
            this._trcFilePathTextBox.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 13);
            this.label3.TabIndex = 32;
            this.label3.Text = "Trace (.trc) file path:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Profiling Table:";
            // 
            // _importTableComboBox
            // 
            this._importTableComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._importTableComboBox.FormattingEnabled = true;
            this._importTableComboBox.Location = new System.Drawing.Point(111, 24);
            this._importTableComboBox.Name = "_importTableComboBox";
            this._importTableComboBox.Size = new System.Drawing.Size(251, 21);
            this._importTableComboBox.TabIndex = 30;
            // 
            // _buttonImport
            // 
            this._buttonImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonImport.Location = new System.Drawing.Point(262, 104);
            this._buttonImport.Name = "_buttonImport";
            this._buttonImport.Size = new System.Drawing.Size(100, 23);
            this._buttonImport.TabIndex = 1;
            this._buttonImport.Text = "Import";
            this._buttonImport.UseVisualStyleBackColor = true;
            this._buttonImport.Click += new System.EventHandler(this.ButtonImport_Click);
            // 
            // _groupBoxTrace
            // 
            this._groupBoxTrace.Controls.Add(this._buttonStartNewTrace);
            this._groupBoxTrace.Dock = System.Windows.Forms.DockStyle.Top;
            this._groupBoxTrace.Location = new System.Drawing.Point(0, 0);
            this._groupBoxTrace.Name = "_groupBoxTrace";
            this._groupBoxTrace.Size = new System.Drawing.Size(368, 304);
            this._groupBoxTrace.TabIndex = 43;
            this._groupBoxTrace.TabStop = false;
            this._groupBoxTrace.Text = "Traces";
            // 
            // _buttonStartNewTrace
            // 
            this._buttonStartNewTrace.Location = new System.Drawing.Point(6, 22);
            this._buttonStartNewTrace.Name = "_buttonStartNewTrace";
            this._buttonStartNewTrace.Size = new System.Drawing.Size(100, 23);
            this._buttonStartNewTrace.TabIndex = 0;
            this._buttonStartNewTrace.Text = "Create Trace";
            this._buttonStartNewTrace.UseVisualStyleBackColor = true;
            this._buttonStartNewTrace.Click += new System.EventHandler(this.ButtonStartNewTrace_Click);
            // 
            // _panelLeft
            // 
            this._panelLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._panelLeft.Controls.Add(this._groupBoxReports);
            this._panelLeft.Controls.Add(this._groupBoxNormalization);
            this._panelLeft.Controls.Add(this._groupBoxTable);
            this._panelLeft.Controls.Add(this.groupBoxConnect);
            this._panelLeft.Location = new System.Drawing.Point(3, 3);
            this._panelLeft.Name = "_panelLeft";
            this._panelLeft.Size = new System.Drawing.Size(509, 437);
            this._panelLeft.TabIndex = 40;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 448);
            this.Controls.Add(this.panelConsole);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "MS SQL Profiling";
            this.groupBoxConnect.ResumeLayout(false);
            this.groupBoxConnect.PerformLayout();
            this._groupBoxTable.ResumeLayout(false);
            this._groupBoxTable.PerformLayout();
            this._groupBoxNormalization.ResumeLayout(false);
            this._groupBoxNormalization.PerformLayout();
            this._groupBoxReports.ResumeLayout(false);
            this._groupBoxReports.PerformLayout();
            this.panelConsole.ResumeLayout(false);
            this._panelRight.ResumeLayout(false);
            this._groupBoxImportFile.ResumeLayout(false);
            this._groupBoxImportFile.PerformLayout();
            this._groupBoxTrace.ResumeLayout(false);
            this._panelLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.GroupBox _groupBoxTable;
        private System.Windows.Forms.GroupBox groupBoxConnect;
        private System.Windows.Forms.ComboBox _comboBoxTable;
        private System.Windows.Forms.Label labelTable;
        private System.Windows.Forms.GroupBox _groupBoxReports;
        private System.Windows.Forms.Button _buttonErrorReportCreate;
        private System.Windows.Forms.CheckBox _checkBoxErrorReportStatus;
        private System.Windows.Forms.Label labelErrorReportStatus;
        private System.Windows.Forms.GroupBox _groupBoxNormalization;
        private System.Windows.Forms.Label labelTextKeyStatus;
        private System.Windows.Forms.TextBox _textBoxExpectedEndTime;
        private System.Windows.Forms.TextBox textBoxPreparedRowProgress;
        private System.Windows.Forms.TextBox _textBoxPreparedRowCount;
        private System.Windows.Forms.TextBox _textBoxStartTime;
        private System.Windows.Forms.TextBox _textBoxRowCount;
        private System.Windows.Forms.Label labelPreparedRowProgress;
        private System.Windows.Forms.Label labelStopTime;
        private System.Windows.Forms.Label labelPreparedRowCount;
        private System.Windows.Forms.Label labelStartTime;
        private System.Windows.Forms.Button _buttonStartNormalization;
        private System.Windows.Forms.Label labelRowCount;
        private System.Windows.Forms.Button _buttonDetailReportCreate;
        private System.Windows.Forms.CheckBox _checkBoxDetailReportStatus;
        private System.Windows.Forms.Label labelDetailReportStatus;
        private System.Windows.Forms.Button buttonDeadlockReportCreate;
        private System.Windows.Forms.CheckBox _checkBoxDeadlockReportStatus;
        private System.Windows.Forms.Label labelDeadlockReportStatus;
        private System.Windows.Forms.Panel panelConsole;
		private System.Windows.Forms.Button buttonDetailReportView;
        private System.Windows.Forms.TextBox _dbTextBox;
        private System.Windows.Forms.TextBox _serverTextBox;
        private System.Windows.Forms.Label labelDB;
        private System.Windows.Forms.Label labelSQLServer;
        private System.Windows.Forms.Label _connectedLabel;
        private System.Windows.Forms.Panel _panelLeft;
        private System.Windows.Forms.GroupBox _groupBoxTrace;
        private System.Windows.Forms.Panel _panelRight;
        private System.Windows.Forms.Button _buttonStartNewTrace;
        private System.Windows.Forms.Button _buttonImport;
        private System.Windows.Forms.GroupBox _groupBoxImportFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _importTableComboBox;
        private System.Windows.Forms.TextBox _trcFilePathTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox _forceOverrideCheckBox;
    }
}

