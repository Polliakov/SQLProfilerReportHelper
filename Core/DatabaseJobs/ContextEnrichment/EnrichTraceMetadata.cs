using System;
using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;
using TraceKnife.Core.DbUtils;
using TraceKnife.Core.Models;

namespace TraceKnife.Core.DatabaseJobs.ContextEnrichment
{
    public class EnrichTraceMetadata : IDataPipelineJob
    {
        private readonly Sql _sql;
        private readonly DbObjectsManager _dbObjectsManager;

        public EnrichTraceMetadata(Sql sql, DbObjectsManager dbObjectsManager)
        {
            _sql = sql;
            _dbObjectsManager = dbObjectsManager;
        }

        public async Task RunAsync(IDataPipelineContext context)
        {
            if (!await _dbObjectsManager.IsTableExist(context.MetadataTable))
                throw new InvalidOperationException($"Table {context.MetadataTable} is not exists.");

            var ds = await _sql.QueryDataSetAsync($@"
select top(1) * from [{context.MetadataTable}]");

            if (ds.IsEmpty())
                throw new InvalidOperationException($"Table {context.MetadataTable} is empty.");

            var row = ds.First();
            var metadata = new TraceMetadata
            {
                CpuSum = row.GetValue<int>(nameof(TraceMetadata.CpuSum)),
                DurationSumUs = row.GetValue<long>(nameof(TraceMetadata.DurationSumUs)),
                WritesSum = row.GetValue<long>(nameof(TraceMetadata.WritesSum)),
                ReadsSum = row.GetValue<long>(nameof(TraceMetadata.ReadsSum)),
                RowsCount = row.GetValue<int>(nameof(TraceMetadata.RowsCount)),
                QueriesCount = row.GetValue<int>(nameof(TraceMetadata.QueriesCount)),
            };

            context.TraceMetadata = metadata;
        }
    }
}
