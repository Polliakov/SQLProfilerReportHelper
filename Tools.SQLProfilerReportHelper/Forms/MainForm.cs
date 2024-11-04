namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;
    using TraceKnife.Core.Profiling;
    using TraceKnife.Core.TraceExports;
    using Tools.SQLProfilerReportHelper.Forms;
    using TraceKnife.Common;
    using TraceKnife.Core.Abstractions;
    using TraceKnife.Core.DataPipelines;
    using TraceKnife.Core.DbUtils;
    using TraceKnife.Core.Normalization;
    using TraceKnife.Core.DatabaseJobs.Reports;
    using TraceKnife.Core.DatabaseJobs.ContextEnrichment;
    using TraceKnife.Core.DatabaseJobs.Manipulations;

    public partial class MainForm : Form
    {
        private TraceLoader _traceLoader;
        private DbProfiler _profiler;
        private DbObjectsManager _dbManager;
        private Sql _sql;
        private readonly IDbObjectsOptions _options;

        public MainForm(IDbObjectsOptions options)
        {
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

            _sql = new Sql(
                new SqlConnectionFactory(connData.ConnectionString), 120);

            _dbManager = new DbObjectsManager(_sql);
            _traceLoader = new TraceLoader(_sql);
            _profiler = new DbProfiler(_sql);

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
                    PreferredParallelism = 10,
                    DbObjectsOptions = _options,
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

                var normalizer = new Normalizer(_sql);
                normalizer.Progress += progressHandler;
                await pipeline
                    .Add(new MetadataReport(_sql))
                    .Add(new EnrichTraceMetadata(_sql, _dbManager))
                    .Add(new NormalizationInitialization(_dbManager, _sql))
                    .Add(normalizer)
                    .Execute();
                normalizer.Progress -= progressHandler;

                SetGroupBoxesEnabled(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error message:" +
                    $"\n {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetGroupBoxesEnabled(true);
            }
        }

        private async void ButtonDetailReportCreate_Click(object sender, EventArgs e)
        {
            var tableName = _comboBoxTable.Text;
            var pipeline = new DataPipeline(new DataPipelineContext
            {
                ProcessingTable = tableName,
                PreferredParallelism = 10,
                DbObjectsOptions = _options,
            });
            await pipeline
                .Add(new EnrichTraceMetadata(_sql, _dbManager))
                .Add(new CreateGroupAndSearchIndexes(_sql, _dbManager))
                .Add(new GroupedReport(_sql))
                .Execute();

            _checkBoxDetailReportStatus.Checked = true;
            _buttonDetailReportCreate.Enabled = false;
        }

        private async void ButtonErrorReportCreate_Click(object sender, EventArgs e)
        {
            var tableName = _comboBoxTable.Text;
            var pipeline = new DataPipeline(new DataPipelineContext
            {
                ProcessingTable = tableName,
                PreferredParallelism = 10,
                DbObjectsOptions = _options,
            });
            await pipeline
                .Add(new ErrorsReport(_sql))
                .Execute();
            _checkBoxErrorReportStatus.Checked = true;
            _buttonErrorReportCreate.Enabled = false;
        }

        private async void ButtonDeadlockReportCreate_Click(object sender, EventArgs e)
        {
            var tableName = _comboBoxTable.Text;
            var pipeline = new DataPipeline(new DataPipelineContext
            {
                ProcessingTable = tableName,
                PreferredParallelism = 10,
                DbObjectsOptions = _options,
            });
            await pipeline
                .Add(new DeadlockReport(_sql))
                .Execute();
            _checkBoxDeadlockReportStatus.Checked = true;
            buttonDeadlockReportCreate.Enabled = false;
        }

        private void ButtonDetailReportView_Click(object sender, EventArgs e)
        {
            MessageBox.Show("In development...", "Experimental",
                MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                var detailExists = await _dbManager.IsTableExist(tableName + _options.TableGroupedPostfix);
                var draftExists = await _dbManager.IsTableExist(tableName + _options.TableDraftPostfix);
                var errorExists = await _dbManager.IsTableExist(tableName + _options.TableErrorsPostfix);

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
