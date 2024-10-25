namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Tools.SQLProfilerReportHelper.Database.Aggregation;
    using Tools.SQLProfilerReportHelper.Database.Common;
    using Tools.SQLProfilerReportHelper.Database.Profiling;
    using Tools.SQLProfilerReportHelper.Database.TraceExports;
    using Tools.SQLProfilerReportHelper.Forms;
    using Tools.SQLProfilerReportHelper.UiKit;

    public partial class MainForm : Form
    {
        public Helper TableUtil { get; set; }

        private TraceLoader _traceLoader;
        private DbProfiler _profiler;
        private DbObjectsManager _dbManager;
        private Normalizer _normalizer;

        private Action<object> _tableInputHandler;

        public MainForm()
        {
            InitializeComponent();
            TableUtil = new Helper();
            backgroundWorkerPrepareTabele.WorkerReportsProgress = true;
            backgroundWorkerPrepareTabele.WorkerSupportsCancellation = true;

            Action<object> table_TextChanged = Table_TextChanged;
            _tableInputHandler = table_TextChanged.Debounce(300);

            SetGroupBoxesEnabled(false);

            _connectedLabel.Visible = false;
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
            _traceLoader = new TraceLoader(s);
            _normalizer = new Normalizer(_dbManager, s);

            SetGroupBoxesEnabled(true);

            _connectedLabel.Visible = true;
            _serverTextBox.Text = connData.DataSource;
            _dbTextBox.Text = connData.InitialCatalog;
        }

        private void SetGroupBoxesEnabled(bool isEnabled)
        {
            _groupBoxNormalization.Enabled = isEnabled;
            _groupBoxReports.Enabled = isEnabled;
            _groupBoxTable.Enabled = isEnabled;
            _groupBoxTrace.Enabled = isEnabled;
            _groupBoxImportFile.Enabled = isEnabled;
        }

        private async void ButtonPrepare_Click(object sender, EventArgs e)
        {
            var tableName = _comboBoxTable.Text;

            if (string.IsNullOrWhiteSpace(tableName))
            {
                MessageBox.Show("Table name is invalid", "Invalid Input", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetGroupBoxesEnabled(false);
            try
            {
                await _normalizer.Prepare(tableName);

                buttonStartSP.Enabled = true;
                buttonStart.Enabled = true;
                EnableAggregationReports();
                _labelInited.Visible = true;

                textBoxRowCount.Text = TableUtil.RowCountForPrepare.ToString();
                textBoxPreparedRowCount.Text = TableUtil.RowCountPrepared.ToString();

                SetGroupBoxesEnabled(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error message:" +
                    $"\n {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetGroupBoxesEnabled(true);
            }
        }

        private async void EnableAggregationReports()
        {
            var tableName = _comboBoxTable.Text;

            var detailExists = await _dbManager.IsTableExist(_dbManager.GetTableNameDetail(tableName));
            var draftExists = await _dbManager.IsTableExist(_dbManager.GetTableNameDraft(tableName));
            var errorExists = await _dbManager.IsTableExist(_dbManager.GetTableNameError(tableName));

            _checkBoxDetailReportStatus.Checked = detailExists;
            _buttonDetailReportCreate.Enabled = !detailExists;
            _checkBoxDraftReportStatus.Checked = draftExists;
            _buttonDraftReportCreate.Enabled = !draftExists;
            _checkBoxErrorReportStatus.Checked = errorExists;
            _buttonErrorReportCreate.Enabled = !errorExists;
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

        private void buttonDetailReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDetailReport();
            _checkBoxDetailReportStatus.Checked = true;
            _buttonDetailReportCreate.Enabled = false;
        }

        private void buttonDraftReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDraftReport();
            _checkBoxDraftReportStatus.Checked = true;
            _buttonDraftReportCreate.Enabled = false;
        }

        private void buttonErrorReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateErrorReport();
            _checkBoxErrorReportStatus.Checked = true;
            _buttonErrorReportCreate.Enabled = false;
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
                TableUtil.StartTime = DateTime.Now;
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

        private void ButtonStartNewTrace_Click(object sender, EventArgs e)
        {
            new CreateTraceForm(_profiler).ShowDialog();
        }

        private async void ButtonImport_Click(object sender, EventArgs e)
        {
            try
            {
                var filePath = _trcFilePathTextBox.Text;
                var tableName = _importTableComboBox.Text;
                var forceOverride = _forceOverrideCheckBox.Checked;

                if (string.IsNullOrEmpty(filePath) ||
                    string.IsNullOrEmpty(tableName))
                {
                    MessageBox.Show("", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SetGroupBoxesEnabled(false);

                await _traceLoader.LoadToDb(filePath, tableName, forceOverride);

                MessageBox.Show("Trace file imported.", "Completed",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                SetGroupBoxesEnabled(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error message:" +
                    $"\n {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetGroupBoxesEnabled(true);
            }
        }

        private void ComboBoxTable_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TableUtil.TableName = _comboBoxTable.Text;
                _buttonPrepare.Enabled = true;
                buttonDeadlockReportCheck.Enabled = true;
            }
            catch
            {
                _buttonPrepare.Enabled = false;
                buttonDeadlockReportCheck.Enabled = false;
            }
        }

        private void Table_TextChanged(object sender)
        {
            // ...
        }
    }
}
