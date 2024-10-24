using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools.SQLProfilerReportHelper.Database.Profiling;

namespace Tools.SQLProfilerReportHelper.Forms
{
    public partial class CreateTraceForm : Form
    {
        private DbProfiler _profiler;

        public CreateTraceForm(DbProfiler dbProfiler)
        {
            _profiler = dbProfiler;
            InitializeComponent();
        }

        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            try
            {
                var traceFolderPath = _folderPathTextBox.Text;
                var duration = (int)_durationNumeric.Value;
                var maxFileSize = (int)_fileSizeNumeric.Value;

                if (string.IsNullOrWhiteSpace(traceFolderPath) ||
                    duration < 1 ||
                    maxFileSize < 1)
                {
                    MessageBox.Show("", "Invalid Input",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _buttonStart.Enabled = false;

                var traceId = await _profiler.StartNewTrace(duration, traceFolderPath, maxFileSize);

                MessageBox.Show($"TraceId: {traceId}", "Trace started",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error message:" +
                    $"\n {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _buttonStart.Enabled = true;
            }
        }
    }
}
