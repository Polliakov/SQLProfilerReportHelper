using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.DatabaseJobs.Reports
{
    public class ErrorsReport : IDataPipelineJob
    {
        private readonly Sql _sql;

        public ErrorsReport(Sql sql)
        {
            _sql = sql;
        }

        public Task RunAsync(IDataPipelineContext context)
        {
            return _sql.ExecuteNonQueryAsync(60 * 60, $@"
SELECT [DatabaseName], [Error], [ApplicationName], [ErrorText], count(*) as [Count], Max([StartTime]) as [StartTime]
INTO [dbo].[{context.ErrorsReportTable}]
FROM
(
	SELECT [DatabaseName], [Error], [ApplicationName], CAST([TextData] as varchar(max)) as [ErrorText], [StartTime]
	FROM [dbo].[{context.ProcessingTable}]
	WHERE EventClass = 162
) [Errors]
GROUP BY [DatabaseName], [Error], [ApplicationName], [ErrorText]
ORDER BY [DatabaseName], [Error], [ApplicationName], [ErrorText]
");
        }
    }
}
