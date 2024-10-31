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
            var options = new ApplicationOptions
            {
                NormalizationFunctionName = "NormalizeTextData0",
                NormalizedTextDataColumn = "TextKey",
                TableDraftPostfix = ".Draft",
                TableDetailPostfix = ".Detailed",
                TableDeadlockPostfix = ".Deadlocks",
                TableErrorPostfix = ".Errors",
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(options));
        }
    }
}
