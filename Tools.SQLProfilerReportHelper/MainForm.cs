namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Tools.SQLProfilerReportHelper.Database.Common;
    using Tools.SQLProfilerReportHelper.Database.Profiling;

    public partial class MainForm : Form
    {
        public Helper TableUtil { get; set; }

        private DbProfiler _profiler;
        private DbObjectsManager _dbManager;

        public MainForm()
        {
            InitializeComponent();
            TableUtil = new Helper();
            backgroundWorkerPrepareTabele.WorkerReportsProgress = true;
            backgroundWorkerPrepareTabele.WorkerSupportsCancellation = true;

            groupBoxFunction.Enabled = false;
            groupBoxPrepare.Enabled = false;
            groupBoxReports.Enabled = false;
            groupBoxTable.Enabled = false;
            _groupBoxTrace.Enabled = false;

            _connectedLabel.Visible = false;
            buttonStart.Enabled = true;
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            var connectForm = new ConnectSqlForm();
            if (connectForm.ShowDialog() != DialogResult.OK)
                return;

            var connData = connectForm.ConnectionData;
            TableUtil.Connect(connData.ConnectionString);

            var f = new SqlConnectionFactory(connData.ConnectionString);
            var s = new Sql(f, 60);
            _profiler = new DbProfiler(f, s);
            _dbManager = new DbObjectsManager(f, s);

            groupBoxFunction.Enabled = true;
            groupBoxPrepare.Enabled = true;
            groupBoxReports.Enabled = true;
            groupBoxTable.Enabled = true;
            _groupBoxTrace.Enabled = true;

            _connectedLabel.Visible = true;
            _serverTextBox.Text = connData.DataSource;
            _dbTextBox.Text = connData.InitialCatalog;
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
            var isExist = TableUtil.IsTableExist(TableUtil.TableNameDraft);
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
            var tableExist = TableUtil.IsTableExist(TableUtil.TableNameDetail);
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
            var tableExist = TableUtil.IsTableExist(TableUtil.TableNameError);
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
            var tableExist = TableUtil.IsTableExist(TableUtil.TableNameDeadlock);
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

        private async void ButtonCheckFunction_Click(object sender, EventArgs e)
        {
            bool functionExist = await _dbManager.IsFunctionExists("NormalizeTextData0");
            checkBoxFunctionExist.Checked = functionExist;
            buttonCreateFunction.Enabled = !functionExist;
        }

        private void buttonCreateFunction_Click(object sender, EventArgs e)
        {
            TableUtil.CreateFunctionNormalizeTextData();
            checkBoxFunctionExist.Checked = true;
            buttonCreateFunction.Enabled = false;
        }

        private async void ButtonStartNewTrace_Click(object sender, EventArgs e)
        {
            try
            {
                var traceId = await _profiler.StartNewTrace(60, "D:\\SqlProfiling\\", 1);
                MessageBox.Show($"TraceId: {traceId}", "Trace started", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error message:" +
                    $"\n {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
