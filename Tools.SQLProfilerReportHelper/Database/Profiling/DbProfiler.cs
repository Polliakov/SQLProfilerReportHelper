using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tools.SQLProfilerReportHelper.Database.Common;

namespace Tools.SQLProfilerReportHelper.Database.Profiling
{
    public class DbProfiler
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly Sql _sql;

        public DbProfiler(SqlConnectionFactory connectionFactory, Sql sql)
        {
            _connectionFactory = connectionFactory;
            _sql = sql;
        }

        public async Task<int> StartNewTrace(
            int duration,
            string reportsFolder,
            int maxFileSizeMB)
        {
            using (var connection = await _connectionFactory.Create())
            {
                var command = new SqlCommand()
                {
                    Connection = connection,
                    CommandTimeout = 60,
                    CommandText = CreateTraceScript(),
                };
                command.Parameters.AddWithValue("@traceDuration", duration);
                command.Parameters.AddWithValue("@reportsFolder", reportsFolder);
                command.Parameters.AddWithValue("@maxFileSizeMB", maxFileSizeMB);

                var reader = await command.ExecuteReaderAsync();
                if (await reader.NextResultAsync())
                {
                    var cols = reader.GetColumnSchema();
                    var resultName = cols.FirstOrDefault()?.ColumnName;
                    if (resultName == "TraceId")
                        return (int)reader[resultName];
                    throw new Exception($"Failed to start trace. ErrorCode: {reader[resultName]}");
                }
                throw new Exception($"Can't get start trace result.");
            }
        }

        public async Task StartTrace(int traceId)
        {
            await _sql.ExecuteNonQueryAsync(
@"EXEC sp_trace_setstatus @traceId, @status = 1",
                new SqlParameter("@traceId", traceId));
        }

        public async Task StopTrace(int traceId)
        {
            await _sql.ExecuteNonQueryAsync(
@"EXEC sp_trace_setstatus @traceId, @status = 0",
                new SqlParameter("@traceId", traceId));
        }

        public async Task DeleteTrace(int traceId)
        {
            await _sql.ExecuteNonQueryAsync(@"
EXEC sp_trace_setstatus @traceId, @status = 0
EXEC sp_trace_setstatus @traceId, @status = 2", 
                new SqlParameter("@traceId", traceId));
        }

        private string CreateTraceScript()
        {
            return @"
set @reportsFolder = rtrim(rtrim(@reportsFolder, '\'), '/')
declare @fileName varchar(max) = CONCAT(@reportsFolder, \trace_', format(getdate(), 'yyyy-MM-dd-HHmmss'))

declare @rc int
declare @TraceId int
declare @stopTraceTime datetime
declare @traceFileName nvarchar(256)
declare @traceOptions int

-- Duration of trace
set @stopTraceTime = DATEADD(second, @traceDuration, SYSDATETIME())
set @traceFileName = @fileName 
-- TRACE_FILE_ROLLOVER
set @traceOptions = 2

exec @rc = sp_trace_create @TraceId output, @traceOptions, @traceFileName, @maxFileSizeMB, @stopTraceTime
if (@rc != 0) goto error

-- Set the events
declare @on bit
set @on = 1

-- 162. User Error Message. Displays error messages that users see in the case of an error or exception.
exec sp_trace_setevent @TraceId, 162, 1, @on  -- TextData. ntext.
exec sp_trace_setevent @TraceId, 162, 4, @on  -- TransactionID. bigint.
exec sp_trace_setevent @TraceId, 162, 9, @on  -- ClientProcessID. int.
exec sp_trace_setevent @TraceId, 162, 10, @on -- ApplicationName. nvarchar.
exec sp_trace_setevent @TraceId, 162, 11, @on -- LoginName. nvarchar.
exec sp_trace_setevent @TraceId, 162, 12, @on -- SPID. int.
exec sp_trace_setevent @TraceId, 162, 20, @on -- Severity.
exec sp_trace_setevent @TraceId, 162, 14, @on -- StartTime. datetime.
exec sp_trace_setevent @TraceId, 162, 31, @on -- Error. int.
exec sp_trace_setevent @TraceId, 162, 35, @on -- DatabaseName. nvarchar.
exec sp_trace_setevent @TraceId, 162, 49, @on -- RequestID. int.
exec sp_trace_setevent @TraceId, 162, 50, @on -- XactSequence. bigint.

-- 148. Deadlock Graph. Occurs when an attempt to acquire a lock is canceled because the attempt was part of a deadlock and was chosen as the deadlock victim. Provides an XML description of a deadlock.
exec sp_trace_setevent @TraceId, 148, 1, @on  -- TextData. ntext.
exec sp_trace_setevent @TraceId, 148, 4, @on  -- TransactionID. bigint. Not used.
exec sp_trace_setevent @TraceId, 148, 11, @on -- LoginName. nvarchar.
exec sp_trace_setevent @TraceId, 148, 12, @on -- SPID. int.
exec sp_trace_setevent @TraceId, 148, 14, @on -- StartTime. datetime.

-- 10. RPC:Completed. Occurs when a remote procedure call (RPC) has completed.
exec sp_trace_setevent @TraceId, 10, 1, @on   -- TextData. ntext.
exec sp_trace_setevent @TraceId, 10, 4, @on   -- TransactionID. bigint.
exec sp_trace_setevent @TraceId, 10, 9, @on   -- ClientProcessID. int.
exec sp_trace_setevent @TraceId, 10, 10, @on  -- ApplicationName. nvarchar.
exec sp_trace_setevent @TraceId, 10, 11, @on  -- LoginName. nvarchar.
exec sp_trace_setevent @TraceId, 10, 12, @on  -- SPID. int.
exec sp_trace_setevent @TraceId, 10, 13, @on  -- Duration. bigint.
exec sp_trace_setevent @TraceId, 10, 14, @on  -- StartTime. datetime.
exec sp_trace_setevent @TraceId, 10, 15, @on  -- EndTime. datetime.
exec sp_trace_setevent @TraceId, 10, 16, @on  -- Reads. bigint.
exec sp_trace_setevent @TraceId, 10, 17, @on  -- Writes. bigint.
exec sp_trace_setevent @TraceId, 10, 18, @on  -- CPU. int.
exec sp_trace_setevent @TraceId, 10, 31, @on  -- Error. int.
exec sp_trace_setevent @TraceId, 10, 34, @on  -- ObjectName. nvarchar.
exec sp_trace_setevent @TraceId, 10, 35, @on  -- DatabaseName. nvarchar.
exec sp_trace_setevent @TraceId, 10, 48, @on  -- RowCounts. bigint.
exec sp_trace_setevent @TraceId, 10, 49, @on  -- RequestID. int.
exec sp_trace_setevent @TraceId, 10, 50, @on  -- XactSequence. bigint.

-- 12. SQL:BatchCompleted. Occurs when a Transact-SQL batch has completed.
exec sp_trace_setevent @TraceId, 12, 1, @on   -- TextData. ntext.
exec sp_trace_setevent @TraceId, 12, 4, @on   -- TransactionID. bigint.
exec sp_trace_setevent @TraceId, 12, 9, @on   -- ClientProcessID. int.
exec sp_trace_setevent @TraceId, 12, 11, @on  -- LoginName. nvarchar.
exec sp_trace_setevent @TraceId, 12, 10, @on  -- ApplicationName. nvarchar.
exec sp_trace_setevent @TraceId, 12, 12, @on  -- SPID. int.
exec sp_trace_setevent @TraceId, 12, 13, @on  -- Duration. bigint.
exec sp_trace_setevent @TraceId, 12, 14, @on  -- StartTime. datetime.
exec sp_trace_setevent @TraceId, 12, 15, @on  -- EndTime. datetime.
exec sp_trace_setevent @TraceId, 12, 16, @on  -- Reads. bigint.
exec sp_trace_setevent @TraceId, 12, 17, @on  -- Writes. bigint.
exec sp_trace_setevent @TraceId, 12, 18, @on  -- CPU. int.
exec sp_trace_setevent @TraceId, 12, 31, @on  -- Error. int.
exec sp_trace_setevent @TraceId, 12, 35, @on  -- DatabaseName. nvarchar.
exec sp_trace_setevent @TraceId, 12, 48, @on  -- RowCounts. bigint.
exec sp_trace_setevent @TraceId, 12, 49, @on  -- RequestID. int.
exec sp_trace_setevent @TraceId, 12, 50, @on  -- XactSequence. bigint.


-- Set the Filters
--exec sp_trace_setfilter @TraceId, 35, 0, 6, @db1

-- Set the trace status to start
exec sp_trace_setstatus @TraceId, 1

-- display trace id for future references
select TraceId=@TraceId
goto finish

error: 
select ErrorCode=@rc

finish: 
";
        }
    }
}
