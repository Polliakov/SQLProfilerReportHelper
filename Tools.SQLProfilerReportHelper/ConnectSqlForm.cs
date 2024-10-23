using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Tools.SQLProfilerReportHelper.Database.Common;

namespace Tools.SQLProfilerReportHelper
{
    public partial class ConnectSqlForm : Form
    {
        public SqlConnectionStringBuilder ConnectionData => _connectionStringBuilder;
        private SqlConnectionStringBuilder _connectionStringBuilder;

        public ConnectSqlForm()
        {
            InitializeComponent();
            _serverComboBox.Text = "DESKTOP-SIBSN99\\SQLEXPRESS";
            _dbComboBox.Text = "PerfomanceTests";
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            var connBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = _serverComboBox.Text,
                InitialCatalog = _dbComboBox.Text,
                TrustServerCertificate = true,
            };
            if (_useWinAuthCheckBox.Checked)
            {
                connBuilder.IntegratedSecurity = true;
            }
            else
            {
                connBuilder.UserID = _loginTextBox.Text;
                connBuilder.Password = _passwordTextBox.Text;
            }

            try
            {
                SqlConnectionFactory.CheckConnect(connBuilder.ConnectionString);
                _connectionStringBuilder = connBuilder;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection to \"{connBuilder.ConnectionString}\" failed\nError message:" +
                    $"\n {ex}", "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UseWinAuthCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _loginTextBox.Text = string.Empty;
            _passwordTextBox.Text = string.Empty;
            _loginTextBox.Enabled = !_loginTextBox.Enabled;
            _passwordTextBox.Enabled = !_passwordTextBox.Enabled;
        }
    }
}
