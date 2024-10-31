using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;
using TraceKnife.Core.DbUtils;

namespace TraceKnife.Core.Normalization
{
    public class Normalizer : IDataPipelineJob<int>
    {
        public event Action<int> Progress;

        private const int _batchSize = 1000;

        private readonly DbObjectsManager _dbManager;
        private readonly Sql _sql;
        private readonly IApplicationOptions _options;

        public Normalizer(DbObjectsManager dbManager, Sql sql, IApplicationOptions options)
        {
            _dbManager = dbManager;
            _sql = sql;
            _options = options;
        }

        public async Task RunAsync(IDataPipelineContext context)
        {
            var countAll = await _dbManager.GetRowsCount(context.ProcessingTable);

            var tasks = new List<Task>(context.PreferredParallelism);

            var byOne = countAll / context.PreferredParallelism;
            foreach (var n in Enumerable.Range(0, context.PreferredParallelism - 1))
            {
                tasks.Add(Normalize(byOne * n, byOne, context));
            }
            var lastOffset = byOne * (context.PreferredParallelism - 1);
            tasks.Add(Normalize(lastOffset, countAll - lastOffset, context));

            await Task.WhenAll(tasks);
        }

        private async Task Normalize(long startFrom, long count, IDataPipelineContext context)
        {
            var take = Math.Min(_batchSize, count);
            var processedCounter = 0L;
            while (processedCounter < count)
            {
                if (count - processedCounter < take)
                    take = processedCounter - count;
                if (take <= 0)
                    return;

                await _sql.ExecuteNonQueryAsync($@"
update [dbo].[{context.ProcessingTable}]
set [TextKey] =	dbo.{_options.NormalizationFunctionName}(CAST([TextData] as varchar(2000)))
where [Id] in (select [Id] from [dbo].[{context.ProcessingTable}] order by [Id]
offset {startFrom} rows fetch next {take} rows only)
");
                startFrom += take;
                processedCounter += take;

                Progress?.Invoke((int)take);
            }
        }
    }
}
