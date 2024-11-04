namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.Windows.Forms;
    using TraceKnife.Core.Configuration;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var options = new DbObjectsOptions
            {
                NormalizationFunctionName = "NormalizeTextData0",
                NormalizedTextDataColumn = "TextKey",
                TableMetadataPostfix = ".Metadata",
                TableDraftPostfix = ".Draft",
                TableGroupedPostfix = ".Grouped",
                TableDeadlockPostfix = ".Deadlocks",
                TableErrorsPostfix = ".Errors",
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(options));
        }
    }
}
