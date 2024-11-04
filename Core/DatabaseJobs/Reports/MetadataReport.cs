using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;
using TraceKnife.Core.Models;

namespace TraceKnife.Core.DatabaseJobs.Reports
{
    public class MetadataReport : IDataPipelineJob
    {
        private readonly Sql _sql;

        public MetadataReport(Sql sql)
        {
            _sql = sql;
        }

        public Task RunAsync(IDataPipelineContext context)
        {
            return _sql.ExecuteNonQueryAsync(60 * 30, $@"
select 
      sum(CPU) as {nameof(TraceMetadata.CpuSum)}
    , sum(Duration) as {nameof(TraceMetadata.DurationSumUs)}
    , sum(Reads) as {nameof(TraceMetadata.ReadsSum)}
    , sum(Writes) as {nameof(TraceMetadata.WritesSum)}
    , count(*) as {nameof(TraceMetadata.RowsCount)}
    , (select count(*) from [dbo].[{context.ProcessingTable}] 
       where EventClass in (10, 12)) as {nameof(TraceMetadata.QueriesCount)}
into [dbo].[{context.MetadataTable}]
from [dbo].[{context.ProcessingTable}]
");
        }
    }
}
