using System.Threading.Tasks;
using TraceKnife.Common;
using TraceKnife.Core.Abstractions;

namespace TraceKnife.Core.DatabaseJobs.Reports
{
    public class DeadlockReport : IDataPipelineJob
    {
        private readonly Sql _sql;

        public DeadlockReport(Sql sql)
        {
            _sql = sql;
        }

        public Task RunAsync(IDataPipelineContext context)
        {
            return _sql.ExecuteNonQueryAsync(60 * 60, $@"
CREATE TABLE [dbo].[{context.DeadlocksReportTable}](
	[RowNumber] [int] IDENTITY(0,1) NOT NULL,
	[EventClass] [int] NULL,
	[LoginName] [nvarchar](128) NULL,
	[SPID] [int] NULL,
	[StartTime] [datetime] NULL,
	[TextData] [ntext] NULL,
	[TransactionID] [bigint] NULL,
	[GroupID] [int] NULL,
	[BinaryData] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[RowNumber] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

INSERT INTO [dbo].[{context.DeadlocksReportTable}]
    ([EventClass]
    ,[LoginName]
    ,[SPID]
    ,[StartTime]
    ,[TextData]
    ,[TransactionID]
    ,[GroupID])
SELECT 
     [EventClass]
    ,[LoginName]
    ,[SPID]
    ,[StartTime]
    ,[TextData]
    ,[TransactionID]
    ,DATALENGTH ( [TextData] ) as [GroupID]
FROM [dbo].[{context.ProcessingTable}]
WHERE [EventClass] = 148
ORDER BY [GroupID]
");
        }
    }
}
