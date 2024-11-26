using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;
using TraceKnife.Core.DbUtils;

namespace TraceKnife.Core.DatabaseJobs.Manipulations
{
    public class CreateGroupAndSearchIndexes : IDataPipelineJob
    {
        private readonly Sql _sql;
        private readonly DbObjectsManager _dbObjectsManager;

        public CreateGroupAndSearchIndexes(Sql sql, DbObjectsManager dbObjectsManager)
        {
            _sql = sql;
            _dbObjectsManager = dbObjectsManager;
        }

        public async Task RunAsync(IDataPipelineContext context)
        {
            var groupingIndex = $"IX_{context.ProcessingTable}_TextKey_DatabaseName_ObjectName";
            var searchIndex = $"IX_{context.ProcessingTable}_TextKey";

            if (!await _dbObjectsManager.IsIndexExists(groupingIndex))
            {
                await _sql.ExecuteNonQueryAsync(60 * 60, $@"
create nonclustered index [{groupingIndex}]
on [dbo].[{context.ProcessingTable}]([DatabaseName],[TextKey],[ObjectName])");
            }

            if (!await _dbObjectsManager.IsIndexExists(searchIndex))
            {
                await _sql.ExecuteNonQueryAsync(60 * 60, $@"
create nonclustered index [{searchIndex}]
on [dbo].[{context.ProcessingTable}]([TextKey])");
            }
        }
    }
}
