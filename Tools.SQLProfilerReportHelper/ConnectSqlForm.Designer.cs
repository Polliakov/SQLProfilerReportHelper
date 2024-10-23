namespace Tools.SQLProfilerReportHelper
{
    partial class ConnectSqlForm
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
            this._groupBoxConnect = new System.Windows.Forms.GroupBox();
            this._passwordTextBox = new System.Windows.Forms.TextBox();
            this._loginTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._useWinAuthCheckBox = new System.Windows.Forms.CheckBox();
            this._buttonConnect = new System.Windows.Forms.Button();
            this._dbComboBox = new System.Windows.Forms.ComboBox();
            this._serverComboBox = new System.Windows.Forms.ComboBox();
            this.labelDB = new System.Windows.Forms.Label();
            this.labelSQLServer = new System.Windows.Forms.Label();
            this._groupBoxConnect.SuspendLayout();
            this.SuspendLayout();
            // 
            // _groupBoxConnect
            // 
            this._groupBoxConnect.Controls.Add(this._passwordTextBox);
            this._groupBoxConnect.Controls.Add(this._loginTextBox);
            this._groupBoxConnect.Controls.Add(this.label1);
            this._groupBoxConnect.Controls.Add(this.label2);
            this._groupBoxConnect.Controls.Add(this._useWinAuthCheckBox);
            this._groupBoxConnect.Controls.Add(this._buttonConnect);
            this._groupBoxConnect.Controls.Add(this._dbComboBox);
            this._groupBoxConnect.Controls.Add(this._serverComboBox);
            this._groupBoxConnect.Controls.Add(this.labelDB);
            this._groupBoxConnect.Controls.Add(this.labelSQLServer);
            this._groupBoxConnect.Location = new System.Drawing.Point(12, 12);
            this._groupBoxConnect.Name = "_groupBoxConnect";
            this._groupBoxConnect.Size = new System.Drawing.Size(390, 211);
            this._groupBoxConnect.TabIndex = 2;
            this._groupBoxConnect.TabStop = false;
            this._groupBoxConnect.Text = "Connect";
            // 
            // _passwordTextBox
            // 
            this._passwordTextBox.Location = new System.Drawing.Point(77, 135);
            this._passwordTextBox.Name = "_passwordTextBox";
            this._passwordTextBox.PasswordChar = '*';
            this._passwordTextBox.Size = new System.Drawing.Size(307, 20);
            this._passwordTextBox.TabIndex = 32;
            // 
            // _loginTextBox
            // 
            this._loginTextBox.Location = new System.Drawing.Point(77, 108);
            this._loginTextBox.Name = "_loginTextBox";
            this._loginTextBox.Size = new System.Drawing.Size(307, 20);
            this._loginTextBox.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 138);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Login:";
            // 
            // _useWinAuthCheckBox
            // 
            this._useWinAuthCheckBox.AutoSize = true;
            this._useWinAuthCheckBox.Location = new System.Drawing.Point(77, 84);
            this._useWinAuthCheckBox.Name = "_useWinAuthCheckBox";
            this._useWinAuthCheckBox.Size = new System.Drawing.Size(159, 17);
            this._useWinAuthCheckBox.TabIndex = 28;
            this._useWinAuthCheckBox.Text = "Use windows authentication";
            this._useWinAuthCheckBox.UseVisualStyleBackColor = true;
            this._useWinAuthCheckBox.CheckedChanged += new System.EventHandler(this.UseWinAuthCheckBox_CheckedChanged);
            // 
            // _buttonConnect
            // 
            this._buttonConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._buttonConnect.Location = new System.Drawing.Point(285, 182);
            this._buttonConnect.Name = "_buttonConnect";
            this._buttonConnect.Size = new System.Drawing.Size(99, 23);
            this._buttonConnect.TabIndex = 27;
            this._buttonConnect.Text = "Connect";
            this._buttonConnect.UseVisualStyleBackColor = true;
            this._buttonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // _dbComboBox
            // 
            this._dbComboBox.FormattingEnabled = true;
            this._dbComboBox.Location = new System.Drawing.Point(77, 56);
            this._dbComboBox.Name = "_dbComboBox";
            this._dbComboBox.Size = new System.Drawing.Size(307, 21);
            this._dbComboBox.TabIndex = 26;
            // 
            // _serverComboBox
            // 
            this._serverComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this._serverComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this._serverComboBox.FormattingEnabled = true;
            this._serverComboBox.Location = new System.Drawing.Point(77, 29);
            this._serverComboBox.Name = "_serverComboBox";
            this._serverComboBox.Size = new System.Drawing.Size(307, 21);
            this._serverComboBox.Sorted = true;
            this._serverComboBox.TabIndex = 25;
            // 
            // labelDB
            // 
            this.labelDB.AutoSize = true;
            this.labelDB.Location = new System.Drawing.Point(15, 59);
            this.labelDB.Name = "labelDB";
            this.labelDB.Size = new System.Drawing.Size(56, 13);
            this.labelDB.TabIndex = 24;
            this.labelDB.Text = "Database:";
            // 
            // labelSQLServer
            // 
            this.labelSQLServer.AutoSize = true;
            this.labelSQLServer.Location = new System.Drawing.Point(6, 32);
            this.labelSQLServer.Name = "labelSQLServer";
            this.labelSQLServer.Size = new System.Drawing.Size(65, 13);
            this.labelSQLServer.TabIndex = 23;
            this.labelSQLServer.Text = "SQL Server:";
            // 
            // ConnectSqlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 235);
            this.Controls.Add(this._groupBoxConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConnectSqlForm";
            this.Text = "Connect Server";
            this._groupBoxConnect.ResumeLayout(false);
            this._groupBoxConnect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _groupBoxConnect;
        private System.Windows.Forms.Button _buttonConnect;
        private System.Windows.Forms.ComboBox _dbComboBox;
        private System.Windows.Forms.ComboBox _serverComboBox;
        private System.Windows.Forms.Label labelDB;
        private System.Windows.Forms.Label labelSQLServer;
        private System.Windows.Forms.CheckBox _useWinAuthCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _passwordTextBox;
        private System.Windows.Forms.TextBox _loginTextBox;
    }
}