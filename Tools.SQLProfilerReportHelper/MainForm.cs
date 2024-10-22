namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        class ConnectionParam
        {
            public string SqlServer { get; set; }
            public string Database { get; set; }
            public string ConnectionString { get { return string.Format("{0}\t{1}", SqlServer.Trim(), Database.Trim()); } }
            public void Parse(string connectionString)
            {
                char[] splitters = { '\t', ' ' };
                string[] connectionStringParts = connectionString.Split(splitters, 2, StringSplitOptions.RemoveEmptyEntries);
                SqlServer = Database = string.Empty;
                if (connectionStringParts.Length >= 1)
                {
                    SqlServer = connectionStringParts[0];
                }
                if (connectionStringParts.Length >= 2)
                {
                    Database = connectionStringParts[1];
                }
            }
        }

        Helper TableUtil { get; set; }
        string SettingsFolderName { get { return "SQLProfilerReportHelper"; } }
        string SettingsFileName { get { return "resentConnectionParams.txt"; } }
        List<ConnectionParam> ConnectionParameters { get; set; }

        public MainForm()
        {
            InitializeComponent();
            TableUtil = new Helper();
            backgroundWorkerPrepareTabele.WorkerReportsProgress = true;
            backgroundWorkerPrepareTabele.WorkerSupportsCancellation = true;
            loadConnectionParamSettings();
            comboBoxSQLServer.AutoCompleteCustomSource.Clear();
            comboBoxSQLServer.Items.Clear();
            comboBoxDB.AutoCompleteCustomSource.Clear();
            comboBoxDB.Items.Clear();

            foreach (ConnectionParam option in ConnectionParameters)
            {
                comboBoxSQLServer.AutoCompleteCustomSource.Add(option.SqlServer);
                comboBoxSQLServer.Items.Add(option.SqlServer);

                comboBoxDB.AutoCompleteCustomSource.Add(option.Database);
                comboBoxDB.Items.Add(option.Database);
            }
            buttonStart.Enabled = true;
        }

        private void saveConnectionParamSettings(ConnectionParam connection)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string settingsFolderPath = Path.Combine(appDataPath, SettingsFolderName);
            if (!Directory.Exists(settingsFolderPath))
            {
                Directory.CreateDirectory(settingsFolderPath);
            }
            string settingsFilePath = Path.Combine(settingsFolderPath, SettingsFileName);
            if (!File.Exists(settingsFilePath))
            {
                StreamWriter writer = File.CreateText(settingsFilePath);
                writer.Close();
            }
            string[] settings = File.ReadAllLines(settingsFilePath);
            if (!settings.Contains(connection.ConnectionString, StringComparer.InvariantCultureIgnoreCase))
            {
                File.AppendAllText(settingsFilePath, connection.ConnectionString);
            }
        }

        private void loadConnectionParamSettings()
        {
            ConnectionParameters = new List<ConnectionParam>();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string settingsFolderPath = Path.Combine(appDataPath, SettingsFolderName);
            string settingsFilePath = Path.Combine(settingsFolderPath, SettingsFileName);
            if (File.Exists(settingsFilePath))
            {
                string[] settings = File.ReadAllLines(settingsFilePath);
                foreach (string connectionParam in settings)
                {
                    ConnectionParam param = new ConnectionParam();
                    param.Parse(connectionParam);
                    ConnectionParameters.Add(param);
                }
            }

        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            var sqlServer = comboBoxSQLServer.Text;
            var dataBase = comboBoxDB.Text;

            try
            {
                TableUtil.Connect(sqlServer, dataBase);
                comboBoxTable.Items.Clear();
                comboBoxTable.Items.AddRange(TableUtil.Tables);
                ConnectionParam options = new ConnectionParam()
                {
                    SqlServer = sqlServer,
                    Database = dataBase,
                };
                saveConnectionParamSettings(options);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show(
                    string.Format("Не удалось соединиться с базой данных {0} на сервере {1} и получить список таблиц.\n{2}"
                        , dataBase
                        , sqlServer
                        , ex.Message
                    )
                    , "Ошибка соединения");
            }
        }

        private void buttonTextKeyCheck_Click(object sender, EventArgs e)
        {
            var tableName = comboBoxTable.Text;
            checkBoxTextKeyStatus.Checked = TableUtil.ColumnExistInTable(tableName, "TextKey");
            buttonTextKeyCreate.Enabled = !checkBoxTextKeyStatus.Checked;
            buttonStartSP.Enabled = checkBoxTextKeyStatus.Checked;
            buttonDetailReportCheck.Enabled = checkBoxTextKeyStatus.Checked;
            buttonDraftReportCheck.Enabled = checkBoxTextKeyStatus.Checked;
            buttonErrorStatCheck.Enabled = checkBoxTextKeyStatus.Checked;

            textBoxRowCount.Text = TableUtil.RowCountForPrepare.ToString();
            textBoxPreparedRowCount.Text = TableUtil.RowCountPrepared.ToString();

        }

        private void comboBoxTable_TextChanged(object sender, EventArgs e)
        {
            TableUtil.TableName = comboBoxTable.Text;
            buttonTextKeyCheck.Enabled = true;
            buttonDeadlockReportCheck.Enabled = true;
            buttonMinuteAndSecondCheck.Enabled = true;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerPrepareTabele.IsBusy != true)
            {
                TableUtil.RowCountForPrepare = TableUtil.GetRowCountForPrepare();
                TableUtil.RowCountPrepared = 0;

                textBoxRowCount.Text = TableUtil.RowCountForPrepare.ToString();
                textBoxPreparedRowCount.Text = TableUtil.RowCountPrepared.ToString();
                TableUtil.StartTime = System.DateTime.Now;
                textBoxStartTime.Text = TableUtil.StartTime.ToString();
                textBoxStopTime.Text = TableUtil.ExpectedStopTime.ToString();

                backgroundWorkerPrepareTabele.RunWorkerAsync();

                buttonStart.Enabled = false;
                buttonStop.Enabled = true;

                buttonStartSP.Enabled = false;
                buttonStopSP.Enabled = false;
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerPrepareTabele.WorkerSupportsCancellation == true)
            {
                backgroundWorkerPrepareTabele.CancelAsync();

                buttonStop.Enabled = false;
                buttonStart.Enabled = true;

                buttonStopSP.Enabled = false;
                buttonStartSP.Enabled = true;
            }
        }

        private void backgroundWorkerPrepareTabele_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                TableUtil.DropIndexOnTextKeys();
            }
            catch (Exception ex)
            { }

            while (TableUtil.PreparedIsComplete == false)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    TableUtil.PrepareTextKeys();
                    worker.ReportProgress(100 * TableUtil.RowCountPrepared / TableUtil.RowCountForPrepare);
                }
            }
            if (TableUtil.PreparedIsComplete)
            {
                worker.ReportProgress(100);
            }
        }

        private void backgroundWorkerPrepareTabele_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBoxPreparedRowCount.Text = TableUtil.RowCountPrepared.ToString();
            textBoxPreparedRowProgress.Text = string.Format("{0} %", e.ProgressPercentage.ToString());
            textBoxStopTime.Text = TableUtil.ExpectedStopTime.ToString();
        }

        private void buttonDraftReportCheck_Click(object sender, EventArgs e)
        {
            var isExist = TableUtil.TableExist(TableUtil.TableNameDraft);
            checkBoxDraftReportStatus.Checked = isExist;
            buttonDraftReportCreate.Enabled = !isExist;
        }

        private void buttonTextKeyCreate_Click(object sender, EventArgs e)
        {
            //TableUtil.CreateIndexes();
            TableUtil.CreateTextKey();
            buttonTextKeyCheck_Click(sender, e);
        }

        private void buttonDetailReportCheck_Click(object sender, EventArgs e)
        {
            var tableExist = TableUtil.TableExist(TableUtil.TableNameDetail);
            checkBoxDetailReportStatus.Checked = tableExist;
            buttonDetailReportCreate.Enabled = !checkBoxDetailReportStatus.Checked;
        }

        private void buttonDetailReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDetailReport();
            checkBoxDetailReportStatus.Checked = true;
            buttonDetailReportCreate.Enabled = false;
        }

        private void buttonDraftReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDraftReport();
            checkBoxDraftReportStatus.Checked = true;
            buttonDraftReportCreate.Enabled = false;
        }

        private void buttonErrorStatCheck_Click(object sender, EventArgs e)
        {
            var tableExist = TableUtil.TableExist(TableUtil.TableNameError);
            checkBoxErrorReportStatus.Checked = tableExist;
            buttonErrorReportCreate.Enabled = !tableExist;
        }

        private void buttonErrorReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateErrorReport();
            checkBoxErrorReportStatus.Checked = true;
            buttonErrorReportCreate.Enabled = false;
        }

        private void buttonDeadlockReportCheck_Click(object sender, EventArgs e)
        {
            var tableExist = TableUtil.TableExist(TableUtil.TableNameDeadlock);
            checkBoxDeadlockReportStatus.Checked = tableExist;
            buttonDeadlockReportCreate.Enabled = !tableExist;
        }

        private void buttonDeadlockReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDeadlockReport();
            checkBoxDeadlockReportStatus.Checked = true;
            buttonDeadlockReportCreate.Enabled = false;
        }

        private void buttonMinuteAndSecondCheck_Click(object sender, EventArgs e)
        {
            var columnsExist = TableUtil.ColumnExistInTable(TableUtil.TableName, "Second01");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Second05");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Second10");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Munute01");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Munute02");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Munute03");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Munute04");
            columnsExist = columnsExist && TableUtil.ColumnExistInTable(TableUtil.TableName, "Munute05");

            checkBoxMinuteAndSecondStatus.Checked = columnsExist;
            buttonMinuteAndSecondCreate.Enabled = !columnsExist;
        }

        private void buttonMinuteAndSecondCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateMinuteAndSecondColumn();
            TableUtil.FillMinuteAndSecondColumn();
            buttonMinuteAndSecondCheck_Click(sender, e);
        }

        private void DisableAllButtons()
        {
            buttonMinuteAndSecondCheck.Enabled = false;
            buttonConnect.Enabled = false;
            buttonDeadlockReportCheck.Enabled = false;
            buttonDeadlockReportCreate.Enabled = false;
            buttonDetailReportCheck.Enabled = false;
            buttonDetailReportCreate.Enabled = false;
            buttonDraftReportCheck.Enabled = false;
            buttonDraftReportCreate.Enabled = false;
            buttonErrorReportCreate.Enabled = false;
            buttonErrorStatCheck.Enabled = false;
            buttonMinuteAndSecondCheck.Enabled = false;
            buttonMinuteAndSecondCreate.Enabled = false;
            buttonStart.Enabled = false;
            buttonStop.Enabled = false;
            buttonTextKeyCheck.Enabled = false;
            buttonTextKeyCreate.Enabled = false;
        }

        private void CheckEnableAllButtons(object sender, EventArgs e)
        {
            buttonDeadlockReportCheck.Enabled = true;
            buttonDeadlockReportCheck_Click(sender, e);

            buttonMinuteAndSecondCheck.Enabled = true;
            buttonMinuteAndSecondCheck_Click(sender, e);
        }

        private void backgroundWorkerPrepareTabele_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStartSP.Enabled = true;

            buttonStop.Enabled = false;
            buttonStopSP.Enabled = false;


            try
            {
                TableUtil.CreateIndexOnTextKeys();
            }
            catch (Exception ex)
            {
            }
        }

        private void backgroundWorkerPrepareTableSP_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            try
            {
                TableUtil.DropIndexOnTextKeys();
            }
            catch (Exception ex)
            { }

            while (!TableUtil.PreparedIsCompleteSP)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    // Perform a time consuming operation and report progress.
                    TableUtil.PrepareTextKeysSP();
                    worker.ReportProgress(100 * TableUtil.RowCountPreparedSP / TableUtil.RowCountForPrepareSP);
                }
            }
            if (TableUtil.PreparedIsCompleteSP)
            {
                worker.ReportProgress(100);
            }
        }

        private void buttonStartSP_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerPrepareTableSP.IsBusy != true)
            {
                TableUtil.RowCountForPrepareSP = TableUtil.GetRowCountForPrepareSP();
                TableUtil.RowCountPreparedSP = 0;

                textBoxRowCount.Text = TableUtil.RowCountForPrepareSP.ToString();
                textBoxPreparedRowCount.Text = TableUtil.RowCountPreparedSP.ToString();
                TableUtil.StartTime = System.DateTime.Now;
                textBoxStartTime.Text = TableUtil.StartTime.ToString();
                textBoxStopTime.Text = TableUtil.ExpectedStopTimeSP.ToString();

                backgroundWorkerPrepareTableSP.RunWorkerAsync();

                buttonStartSP.Enabled = false;
                buttonStopSP.Enabled = true;

                buttonStart.Enabled = false;
                buttonStop.Enabled = false;
            }
        }

        private void buttonStopSP_Click(object sender, EventArgs e)
        {
            if (backgroundWorkerPrepareTableSP.WorkerSupportsCancellation == true)
            {
                backgroundWorkerPrepareTableSP.CancelAsync();

                buttonStop.Enabled = false;
                buttonStart.Enabled = true;

                buttonStopSP.Enabled = false;
                buttonStartSP.Enabled = true;
            }

        }

        private void backgroundWorkerPrepareTableSP_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            textBoxPreparedRowCount.Text = TableUtil.RowCountPreparedSP.ToString();
            textBoxPreparedRowProgress.Text = string.Format("{0} %", e.ProgressPercentage.ToString());
            textBoxStopTime.Text = TableUtil.ExpectedStopTimeSP.ToString();
        }

        private void backgroundWorkerPrepareTableSP_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonStart.Enabled = true;
            buttonStartSP.Enabled = true;

            buttonStop.Enabled = false;
            buttonStopSP.Enabled = false;

        }

        private void buttonDetailReportView_Click(object sender, EventArgs e)
        {
            ReportViewForm reportView = new ReportViewForm(TableUtil);
            reportView.LoadDetailStat(
                TableUtil.GetDetailStat()
                );
            reportView.Show();
        }

        private void buttonCheckFunction_Click(object sender, EventArgs e)
        {
            bool functionExist = TableUtil.FunctionExists("PrepareTextData4");
            checkBoxFunctionExist.Checked = functionExist;
            buttonCreateFunction.Enabled = !functionExist;
        }

        private void buttonCreateFunction_Click(object sender, EventArgs e)
        {
            TableUtil.CreateFunctionPrepareTextData();
            checkBoxFunctionExist.Checked = true;
            buttonCreateFunction.Enabled = false;
        }
    }
}
