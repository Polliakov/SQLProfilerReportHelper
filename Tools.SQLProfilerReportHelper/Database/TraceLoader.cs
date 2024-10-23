using System.Data.SqlClient;
using System.Threading.Tasks;
using Tools.SQLProfilerReportHelper.Database.Common;

namespace Tools.SQLProfilerReportHelper.Database
{
    internal class TraceLoader
    {
        private readonly Sql _sql;

        public TraceLoader(Sql sql)
        {
            _sql = sql;
        }

        public async Task Load(string filePath, string tableName, bool forceOverride = false)
        {
            if (forceOverride)
                await _sql.ExecuteNonQueryAsync($@"
if OBJECT_ID('dbo.[{tableName}]', 'U') is not null
	drop table [{tableName}]");

            await _sql.ExecuteNonQueryAsync(@"
declare @paramDefinition nvarchar(255) = N'@file varchar(255)';
declare @query nvarchar(4000) = '
select
	  [EventClass]
	, [TextData]
	, [Duration]
	, [StartTime]
	, [Reads]
	, [Writes]
	, [CPU]
	, [Error]
	, [ObjectName]
	, [DatabaseName]
	, [TransactionID]
	, [ClientProcessID]
	, [ApplicationName]
	, [LoginName]
	, [SPID]
	, [EndTime]
	, [RowCounts]
	, [RequestID]
	, [XactSequence]
into [' + @traceTableName + ']
from ::fn_trace_gettable(@file, default)';

EXECUTE sp_executesql @query, @paramDefinition, @file = @filePath;",
                new SqlParameter("@traceTableName", tableName),
                new SqlParameter("@filePath", filePath));
        }
    }
}
