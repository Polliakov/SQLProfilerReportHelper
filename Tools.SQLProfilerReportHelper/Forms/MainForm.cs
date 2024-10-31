namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    using Tools.SQLProfilerReportHelper.Database.Profiling;
    using Tools.SQLProfilerReportHelper.Database.TraceExports;
    using Tools.SQLProfilerReportHelper.Forms;
    using TraceKnife.Common;
    using TraceKnife.Core.Abstractions;
    using TraceKnife.Core.DataPipelines;
    using TraceKnife.Core.DbUtils;
    using TraceKnife.Core.Normalization;

    public partial class MainForm : Form
    {
        public Helper TableUtil { get; set; }

        private TraceLoader _traceLoader;
        private DbProfiler _profiler;
        private DbObjectsManager _dbManager;
        private Normalizer _normalizer;
        private NormalizationInitialization _normalizerIniter;
        private readonly IApplicationOptions _options;


        public MainForm(IApplicationOptions options)
        {
            TableUtil = new Helper();
            _options = options;

            InitializeComponent();
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
            var s = new Sql(f, 120);
            _profiler = new DbProfiler(f, s);
            _dbManager = new DbObjectsManager(s);
            _traceLoader = new TraceLoader(s);
            _normalizer = new Normalizer(_dbManager, s, _options);
            _normalizerIniter = new NormalizationInitialization(_dbManager, s, _options);

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

        private async void ButtonStartNormalization_Click(object sender, EventArgs e)
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
                var pipeline = new DataPipeline(new DataPipelineContext
                {
                    ProcessingTable = tableName,
                    PreferredParallelism = 10
                });

                var done = 0;
                var startTime = DateTime.Now;
                var totalRows = await _dbManager.GetRowsCount(tableName);
                _textBoxRowCount.Text = totalRows.ToString();
                _textBoxStartTime.Text = startTime.ToString(CultureInfo.InvariantCulture);

                void progressHandler(int processed)
                {
                    if (processed == 0)
                        return;

                    done += processed;
                    var timePass = DateTime.Now.Subtract(startTime).TotalSeconds;
                    var expectedTime = startTime.AddSeconds(timePass / done * totalRows);

                    _textBoxPreparedRowCount.Text = done.ToString();
                    textBoxPreparedRowProgress.Text = $"{(double)done / totalRows * 100:F2}%";
                    _textBoxExpectedEndTime.Text = expectedTime.ToString(CultureInfo.InvariantCulture);
                }

                _normalizer.Progress += progressHandler;
                await pipeline
                     .AddJob(_normalizerIniter)
                     .AddJob(_normalizer)
                     .Execute();
                _normalizer.Progress -= progressHandler;

                SetGroupBoxesEnabled(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error message:" +
                    $"\n {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetGroupBoxesEnabled(true);
            }
        }

        private void ButtonDetailReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDetailReport();
            _checkBoxDetailReportStatus.Checked = true;
            _buttonDetailReportCreate.Enabled = false;
        }

        private void ButtonErrorReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateErrorReport();
            _checkBoxErrorReportStatus.Checked = true;
            _buttonErrorReportCreate.Enabled = false;
        }

        private void ButtonDeadlockReportCreate_Click(object sender, EventArgs e)
        {
            TableUtil.CreateDeadlockReport();
            _checkBoxDeadlockReportStatus.Checked = true;
            buttonDeadlockReportCreate.Enabled = false;
        }

        private void ButtonDetailReportView_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In development...", "Experimental",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            ReportViewForm reportView = new ReportViewForm(TableUtil);
            reportView.LoadDetailStat(new Model.DetailStat[] { });
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

        private async void ComboBoxTable_TextChanged(object sender, EventArgs e)
        {
            var tableName = _comboBoxTable.Text;
            var tableExists = await _dbManager.IsTableExist(tableName);
            if (tableExists)
            {
                TableUtil.TableName = tableName;

                var detailExists = await _dbManager.IsTableExist(tableName + _options.TableDetailPostfix);
                var draftExists = await _dbManager.IsTableExist(tableName + _options.TableDraftPostfix);
                var errorExists = await _dbManager.IsTableExist(tableName + _options.TableErrorPostfix);

                ToggleReportButtons(detailExists, draftExists, errorExists);

                buttonDeadlockReportCreate.Enabled = true;
                _buttonStartNormalization.Enabled = true;
            }
            else
            {
                ToggleReportButtons(true, true, true);

                buttonDeadlockReportCreate.Enabled = true;
                _buttonStartNormalization.Enabled = false;
            }
        }

        private void ToggleReportButtons(bool detailExists, bool draftExists, bool errorExists)
        {
            _checkBoxDetailReportStatus.Checked = detailExists;
            _buttonDetailReportCreate.Enabled = !detailExists;
            _checkBoxErrorReportStatus.Checked = errorExists;
            _buttonErrorReportCreate.Enabled = !errorExists;
        }
    }
}
