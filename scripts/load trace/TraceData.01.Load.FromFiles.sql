declare @traceTableName nvarchar(255) = 'Trace';
declare @filePath nvarchar(255) = N'D:\SqlProfiling\trace_2024-10-23-115226.trc';

declare @parmDefinition nvarchar(255) = N'@file varchar(255)';
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

EXECUTE sp_executesql @query, @parmDefinition, @file = @filePath;
