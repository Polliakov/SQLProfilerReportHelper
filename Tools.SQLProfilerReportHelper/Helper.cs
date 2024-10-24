﻿namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.SqlClient;

    public class Helper
    {
        private SqlConnection Connection { get; set; }

        string tableName;
        /// <summary>
        /// get/set Имя таблицы с данными профайлинга
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            set
            {
                if (IsTableExist(value))
                {
                    tableName = value;
                }
                else
                {
                    throw new Exception(string.Format("Table {0} not exist", tableName));
                }
            }
        }

        /// <summary>
        /// get Имя таблицы с отчётом по производительности хранимых процедур (быстрый черновой отчёт)
        /// </summary>
        public string TableNameDraft
        {
            get { return TableName + ".DraftStat"; }
        }

        /// <summary>
        /// get Имя таблицы с отчётом по производительности всех SQL-запросов
        /// </summary>
        public string TableNameDetail
        {
            get { return TableName + ".DetailStat"; }
        }

        /// <summary>
        /// get Имя таблицы с отчётом по статистике ошибок
        /// </summary>
        public string TableNameError
        {
            get { return TableName + ".ErrorStat"; }
        }

        /// <summary>
        /// get Имя таблицы с взаимоблокировками
        /// </summary>
        public string TableNameDeadlock
        {
            get { return TableName + ".DeadlockGraphs"; }
        }

        public DateTime StartTime { get; set; }
        public DateTime ExpectedStopTime { get { return GetExpectedStopTime(); } }
        public DateTime ExpectedStopTimeSP { get { return GetExpectedStopTimeSP(); } }

        public bool PreparedIsComplete { get { return (RowCountForPrepare < RowCountPrepared) || RowCountForPrepare == 0; } }

        public bool PreparedIsCompleteSP { get { return (RowCountForPrepareSP < RowCountPreparedSP) || RowCountForPrepareSP == 0; } }

        public string[] Tables { get { return GetTables(); } }

        public int RowCountForPrepare { get; set; }
        public int RowCountPrepared { get; set; }

        public int RowCountForPrepareSP { get; set; }
        public int RowCountPreparedSP { get; set; }

        public void Connect(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();

            RowCountPrepared = 0;
        }

        DateTime GetExpectedStopTime()
        {
            var duration = DateTime.Now.Subtract(StartTime).TotalSeconds;
            var expectedDuration = 0.0;
            if (RowCountPrepared > 0)
                expectedDuration = duration * RowCountForPrepare / RowCountPrepared;
            else
                expectedDuration = duration * RowCountForPrepare / 10;

            return StartTime.AddSeconds(expectedDuration);
        }

        /// <summary>
        /// Расчёт времени, необходимого для завершения операции обработки хранимых процедур.
        /// </summary>
        /// <returns>Момент времени, в который, предположительно операция завершится (время в текущем часовом поясе)</returns>
        /// <remarks>
        /// Метод зависит от внешних переменных.
        /// Предполагается, что:
        /// this.StartTime - момент времени начала операции (время в текущем часовом поясе), изначально DateTime.Now.
        /// this.RowCountPreparedSP - количество уже обработанных строк таблицы, с хранимыми процедурами, изначально 0.
        /// this.RowCountForPrepareSP - количество строк таблицы, которые нужно обработать, изначально равно количеству вызовов хранимых процедур в таблице профайлинга.
        /// </remarks>
		DateTime GetExpectedStopTimeSP()
        {
            //Длительность обработки текущего количества строк (RowCountPreparedSP)
            double duration = DateTime.Now.Subtract(StartTime).TotalSeconds;
            double expectedDuration = 0.0;

            //Длительность обработки полного количества строк (RowCountForPrepareSP) расчитывается по пропорции
            if (RowCountPreparedSP > 0)
                expectedDuration = duration * RowCountForPrepareSP / RowCountPreparedSP;
            else
                //Чтобы не было деления на 0, в качестве первого приближения расчитывается время завершения так,
                //как будто, с момента начала операции уже обраборано 10 строк из полного количества.
                expectedDuration = duration * RowCountForPrepareSP / 10;

            return StartTime.AddSeconds(expectedDuration);
        }

        public void DropIndexOnTextKeys()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 10000;
            command.CommandText = string.Format(@"
DROP NONCLUSTERED INDEX [IX_TraceTable_TextKey_DatabaseName]
ON [dbo].[{0}]
", TableName);
            command.ExecuteNonQuery();
        }

        public void CreateIndexOnTextKeys()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 10000;
            command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_TextKey_DatabaseName]
ON [dbo].[{0}] ([DatabaseName],[TextKey])
", TableName);
            command.ExecuteNonQuery();
        }

        public void PrepareTextKeys()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 10000;
            command.CommandText = string.Format(@"
update TOP (100)
[dbo].[{0}]
set [TextKey] =	dbo.NormalizeTextData0(CAST([TextData] as varchar(550)))
where [TextKey] IS NULL
and ([ObjectName] IS NULL OR [ObjectName] IN ('sp_executesql'))
and [EventClass] IN (10, 12)
", TableName);
            RowCountPrepared += command.ExecuteNonQuery();
        }

        public void PrepareTextKeysSP()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 10000;
            command.CommandText = string.Format(@"
-- Обработка для хранимых процедур
update TOP (100000) [dbo].[{0}]
set [TextKey] =	[ObjectName]
where [ObjectName] IS NOT NULL
and [TextKey] IS NULL
and [ObjectName] NOT IN  ('sp_executesql')
and EventClass in (10, 12)
", TableName);
            RowCountPreparedSP += command.ExecuteNonQuery();
            //			command.CommandText = string.Format(@"
            //-- Предварительная обработка для SQL-запросов
            //update TOP (2000) [dbo].[{0}]
            //set [TextKey] =	NULL
            //where
            //([ObjectName] IS NULL OR [ObjectName] IN ('sp_executesql'))
            //and EventClass in (10, 12)
            //", this.TableName);
            //			this.RowCountPreparedSP += command.ExecuteNonQuery();
        }

        public void CreateIndexes()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;

            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_DatabaseName_EventClass_CPU]
ON [dbo].[{0}] ([DatabaseName],[EventClass])
INCLUDE ([CPU])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_DatabaseName_EventClass_StartTime]
ON [dbo].[{0}] ([DatabaseName],[EventClass])
INCLUDE ([StartTime])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_DatabaseName_EventClass_Reads]
ON [dbo].[{0}] ([DatabaseName],[EventClass])
INCLUDE ([Reads])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_DatabaseName_EventClass_Writes]
ON [dbo].[{0}] ([DatabaseName],[EventClass])
INCLUDE ([Writes])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_DatabaseName_EventClass_Duration]
ON [dbo].[{0}] ([DatabaseName],[EventClass])
INCLUDE ([Duration])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
--CREATE NONCLUSTERED INDEX [IX_TraceTable_DatabaseName_EventClass_DurationReadsWritesCPUTextKey]
--ON [dbo].[{0}] ([DatabaseName],[EventClass])
--INCLUDE ([Duration],[Reads],[Writes],[CPU],[TextKey])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_ObjectName]
ON [dbo].[{0}] ([ObjectName])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_EventClassDurationReadsWritesCPU]
ON [dbo].[{0}] ([EventClass])
INCLUDE ([Duration],[Reads],[Writes],[CPU])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }


            try
            {
                command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTable_EventClassDurationReadsWritesCPUObjectNameDatabaseName]
ON [dbo].[{0}] ([EventClass])
INCLUDE ([Duration],[Reads],[Writes],[CPU],[ObjectName],[DatabaseName])
", TableName);
                command.ExecuteNonQuery();
            }
            catch (Exception) { }

        }

        public void CreateTextKey()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
BEGIN TRANSACTION
ALTER TABLE [dbo].[{0}] ADD
	[TextKey] varchar(1000) NULL
ALTER TABLE [dbo].[{0}] SET (LOCK_ESCALATION = TABLE)
COMMIT
", TableName);
            command.ExecuteNonQuery();
        }

        string[] GetTables()
        {
            string[] tables = { };
            if (Connection.State == System.Data.ConnectionState.Open)
            {
                var command = new SqlCommand();
                command.Connection = Connection;
                command.CommandText = @"select TABLE_NAME
from INFORMATION_SCHEMA.COLUMNS
where COLUMN_NAME = 'EventClass'
ORDER BY TABLE_NAME";
                var reader = command.ExecuteReader();
                var tablesList = new List<string>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tablesList.Add(reader.GetString(0));
                    }
                }
                reader.Close();
                tables = tablesList.ToArray();
            }
            return tables;
        }

        public bool IsTableExist(string tableName)
        {
            bool isExist = false;

            if (Connection.State == System.Data.ConnectionState.Open)
            {
                var command = new SqlCommand();
                command.Connection = Connection;
                command.CommandTimeout = 60;
                command.CommandText = @"select TABLE_NAME
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @tableName
ORDER BY TABLE_NAME";
                command.Parameters.Add(new SqlParameter("@tableName", System.Data.SqlDbType.VarChar, 100)
                {
                    Value = tableName
                }
                    );
                command.Prepare();
                var reader = command.ExecuteReader();
                isExist = reader.HasRows;
                reader.Close();
            }
            return isExist;
        }

        public int GetRowCountForPrepare()
        {
            int rowCount = -1;

            if (Connection.State == System.Data.ConnectionState.Open)
            {
                var command = new SqlCommand();
                command.Connection = Connection;
                command.CommandTimeout = 60 * 60;
                command.CommandText = string.Format(@"
select count(*)
from [dbo].[{0}]
where TextKey IS NULL
and (ObjectName IS NULL OR ObjectName IN ('sp_executesql'))
and EventClass in (10, 12)", TableName);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        rowCount = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }
            return rowCount;
        }

        public int GetRowCountForPrepareSP()
        {
            int rowCount = -1;

            if (Connection.State == System.Data.ConnectionState.Open)
            {
                var command = new SqlCommand();
                command.Connection = Connection;
                command.CommandTimeout = 60 * 60;
                command.CommandText = string.Format(@"
select count(*)
from [dbo].[{0}]
where [ObjectName] IS NOT NULL
and [TextKey] IS NULL
and [ObjectName] NOT IN  ('sp_executesql')
and EventClass in (10, 12)
", TableName);
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        rowCount = reader.GetInt32(0);
                    }
                }
                reader.Close();
            }
            return rowCount;
        }

        public void CreateDeadlockReport()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
CREATE TABLE [dbo].[{0}](
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
", TableNameDeadlock);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
INSERT INTO [dbo].[{1}]
    ([EventClass]
    ,[LoginName]
    ,[SPID]
    ,[StartTime]
    ,[TextData]
    ,[TransactionID]
    ,[GroupID]
    ,[BinaryData])
SELECT [EventClass]
      ,[LoginName]
      ,[SPID]
      ,[StartTime]
      ,[TextData]
      ,[TransactionID]
      ,DATALENGTH ( [TextData] ) as [GroupID]
      ,[BinaryData]
FROM [dbo].[{0}]
WHERE [EventClass] = 148
ORDER BY [GroupID]
", TableName, TableNameDeadlock);
            command.ExecuteNonQuery();
        }


        public void CreateDetailReport()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 160 * 60;
            command.CommandText = string.Format(@"
declare @CPUSumm int; 
declare @DurationSumm float; 
declare @ReadsSumm float;
declare @WritesSumm float;
declare @CountSumm float;

select @CPUSumm = SUM(CPU)
     , @DurationSumm = SUM(Duration)
     , @ReadsSumm = SUM(Reads)
     , @WritesSumm = SUM(Writes)
     , @CountSumm = count(*)
from [dbo].[{0}] where EventClass in (10, 12)


select
	[DatabaseName]
	, LEFT([TextKey], 40) as [TextKey-key]
	, [avg(CPU)] as [avg(CPU)-key]
	, [avg(Duration)] as [avg(Duration)-key]
	, [% Duration] as [% Duration-key]
	, [avg(Reads)] as [avg(Reads)-key]
	, [Count] as [Count-key]

	, [TextKey]

	, [min(CPU)]
	, [avg(CPU)]
	, [max(CPU)]
	, [sum(CPU)]
	, [% CPU]

	, [min(Duration)]
	, [avg(Duration)]
	, [max(Duration)]
	, [sum(Duration)]
	, [% Duration]

	, [min(Reads)]
	, [avg(Reads)]
	, [max(Reads)]
	, [sum(Reads)]
	, [% Reads]

	, [min(Writes)]
	, [avg(Writes)]
	, [max(Writes)]
	, [sum(Writes)]
	, [% Writes]

	, [Count]
	, [% Count]

	, [TextData-min(Duration)]
	, [TextData-max(Duration)]
	, [TextData-min(Reads)]
	, [TextData-max(Reads)]
	, [TextData-min(CPU)]
	, [TextData-max(CPU)]
	, [TextData-min(Writes)]
	, [TextData-max(Writes)]

	, [min(Duration)raw]
	, [max(Duration)raw]

INTO [dbo].[{1}]
from
(
	select
		--Быстрая статистика, для вставки в отчёт по тестированию
		*
		, round(cast([sum(CPU)] as float) / @CPUSumm * 100, 3) as [% CPU]

		, [min(Duration)raw]/1000 as [min(Duration)]
		, [avg(Duration)raw]/1000 as [avg(Duration)]
		, [max(Duration)raw]/1000 as [max(Duration)] 
		, [sum(Duration)raw]/1000 as [sum(Duration)]

		, case 
            when @DurationSumm > 0 
            then round(cast([sum(Duration)raw] as float) / @DurationSumm * 100, 3)
            else 0 
          end as [% Duration]

		, case
            when @ReadsSumm > 0
            then round(cast([sum(Reads)] as float) / @ReadsSumm * 100, 3)
            else 0
          end as [% Reads]

		, case
            when @WritesSumm > 0
            then round(cast([sum(Writes)] as float) / @WritesSumm * 100, 3)
            else 0
          end as [% Writes]

		, case
            when @CountSumm > 0
            then round([Count] / @CountSumm * 100, 3)
            else 0
          end as [% Count]

		, (select top 1 [TextData] from [{0}]) as [TextData-min(Duration)]
		, (select top 1 [TextData] from [{0}]) as [TextData-max(Duration)]
		, (select top 1 [TextData] from [{0}]) as [TextData-min(CPU)]
		, (select top 1 [TextData] from [{0}]) as [TextData-max(CPU)]
		, (select top 1 [TextData] from [{0}]) as [TextData-min(Reads)]
		, (select top 1 [TextData] from [{0}]) as [TextData-max(Reads)]
		, (select top 1 [TextData] from [{0}]) as [TextData-min(Writes)]
		, (select top 1 [TextData] from [{0}]) as [TextData-max(Writes)]

	from
	(
		select
			--Быстрая статистика, для вставки в отчёт по тестированию
			[DatabaseName],
			[TextKey],
  
			--Детальная статистика
			min(CPU) as [min(CPU)], 
			avg(CPU) as [avg(CPU)], 
			max(CPU) as [max(CPU)], 
			sum(CPU) as [sum(CPU)], 

			min(Duration) as [min(Duration)raw], 
			avg(Duration) as [avg(Duration)raw], 
			max(Duration) as [max(Duration)raw], 
			sum(Duration) as [sum(Duration)raw],

			min(Reads) as [min(Reads)], 
			avg(Reads) as [avg(Reads)],
			max(Reads) as [max(Reads)], 
			sum(Reads) as [sum(Reads)], 

			min(Writes) as [min(Writes)], 
			avg(Writes) as [avg(Writes)],
			max(Writes) as [max(Writes)], 
			sum(Writes) as [sum(Writes)], 

			count(*) as [Count]
		from
			[dbo].[{0}] as TTT -- Таблица, в которую сохранили трейс. 
		where
			EventClass in (10, 12)
		group by
			[DatabaseName], [TextKey]
	) as [Statistic]
) as [Statistic2]
order by [% Duration] desc
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            try
            {
                CreateIndexOnTextKeys();
            }
            catch (Exception ex)
            {
            }

            command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTableDetailStat_TextKey_DatabaseName]
ON [dbo].[{0}] ([DatabaseName],[TextKey])
", TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-min(Duration)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Duration = [dbo].[{1}].[min(Duration)raw])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-max(Duration)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Duration = [dbo].[{1}].[max(Duration)raw])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-min(CPU)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and CPU = [dbo].[{1}].[min(CPU)])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-max(CPU)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and CPU = [dbo].[{1}].[max(CPU)])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-min(Reads)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Reads = [dbo].[{1}].[min(Reads)])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-max(Reads)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Reads = [dbo].[{1}].[max(Reads)])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-min(Writes)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Writes = [dbo].[{1}].[min(Writes)])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
UPDATE [dbo].[{1}] SET [TextData-max(Writes)] = 
(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Writes = [dbo].[{1}].[max(Writes)])
    ", TableName, TableNameDetail);
            command.ExecuteNonQuery();
        }

        public void CreateDraftReport()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
declare @CPUSumm int; 
declare @DurationSumm float; 
declare @ReadsSumm float;
declare @WritesSumm float;
declare @CountSumm float;

select @CPUSumm = SUM(CPU)
     , @DurationSumm = SUM(Duration)
     , @ReadsSumm = SUM(Reads)
     , @WritesSumm = SUM(Writes)
     , @CountSumm = count(*)
from [dbo].[{0}] where EventClass in (10, 12)


select
	[DatabaseName]
	, [ObjectName] as [ObjectName-key]
	, [avg(CPU)] as [avg(CPU)-key]
	, [avg(Duration)] as [avg(Duration)-key]
	, [% Duration] as [% Duration-key]
	, [avg(Reads)] as [avg(Reads)-key]
	, [Count] as [Count-key]

	, [ObjectName]

	, [min(CPU)]
	, [avg(CPU)]
	, [max(CPU)]
	, [sum(CPU)]
	, [% CPU]

	, [min(Duration)]
	, [avg(Duration)]
	, [max(Duration)]
	, [sum(Duration)]
	, [% Duration]

	, [min(Reads)]
	, [avg(Reads)]
	, [max(Reads)]
	, [sum(Reads)]
	, [% Reads]

	, [min(Writes)]
	, [avg(Writes)]
	, [max(Writes)]
	, [sum(Writes)]
	, [% Writes]

	, [Count]
	, [% Count]

	, [TextData-min(Duration)]
	, [TextData-max(Duration)]
	, [TextData-min(Reads)]
	, [TextData-max(Reads)]
	, [TextData-min(CPU)]
	, [TextData-max(CPU)]
	, [TextData-min(Writes)]
	, [TextData-max(Writes)]
INTO [dbo].[{1}]
from
(
	select
		--Быстрая статистика, для вставки в отчёт по тестированию
		*
		, round(cast([sum(CPU)] as float) / @CPUSumm * 100, 3) as [% CPU]

		, [min(Duration)raw]/1000 as [min(Duration)]
		, [avg(Duration)raw]/1000 as [avg(Duration)]
		, [max(Duration)raw]/1000 as [max(Duration)] 
		, [sum(Duration)raw]/1000 as [sum(Duration)]
		, round(cast([sum(Duration)raw] as float) / @DurationSumm * 100, 3) as [% Duration]

		, round(cast([sum(Reads)] as float) / @ReadsSumm * 100, 3) as [% Reads]

		, round(cast([sum(Writes)] as float) / @WritesSumm * 100, 3) as [% Writes]

		, round([Count] / @CountSumm * 100, 3) as [% Count]

		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and Duration = [Statistic].[min(Duration)raw]) as [TextData-min(Duration)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and Duration = [Statistic].[max(Duration)raw]) as [TextData-max(Duration)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and CPU = [Statistic].[min(CPU)]) as [TextData-min(CPU)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and CPU = [Statistic].[max(CPU)]) as [TextData-max(CPU)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and Reads = [Statistic].[min(Reads)]) as [TextData-min(Reads)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and Reads = [Statistic].[max(Reads)]) as [TextData-max(Reads)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and Writes = [Statistic].[min(Writes)]) as [TextData-min(Writes)]
		,(select top 1 [TextData] from [{0}] where [ObjectName] = [Statistic].[ObjectName] and [DatabaseName] = [Statistic].[DatabaseName] and Writes = [Statistic].[max(Writes)]) as [TextData-max(Writes)]

	from
	(
		select
			--Быстрая статистика, для вставки в отчёт по тестированию
			[DatabaseName],
			[ObjectName],
  
			--Детальная статистика
			min(CPU) as [min(CPU)], 
			avg(CPU) as [avg(CPU)], 
			max(CPU) as [max(CPU)], 
			sum(CPU) as [sum(CPU)], 

			min(Duration) as [min(Duration)raw], 
			avg(Duration) as [avg(Duration)raw], 
			max(Duration) as [max(Duration)raw], 
			sum(Duration) as [sum(Duration)raw],

			min(Reads) as [min(Reads)], 
			avg(Reads) as [avg(Reads)],
			max(Reads) as [max(Reads)], 
			sum(Reads) as [sum(Reads)], 

			min(Writes) as [min(Writes)], 
			avg(Writes) as [avg(Writes)],
			max(Writes) as [max(Writes)], 
			sum(Writes) as [sum(Writes)], 

			count(*) as [Count]
		from
			[dbo].[{0}] as TTT -- Таблица, в которую сохранили трейс. 
		where
			EventClass in (10, 12)
		group by
			[DatabaseName], [ObjectName]
	) as [Statistic]
) as [Statistic2]
order by [% Duration] desc
    ", TableName, TableNameDraft);
            command.ExecuteNonQuery();
        }

        public void CreateErrorReport()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
SELECT [DatabaseName], [Error], [ApplicationName], [ErrorText], count(*) as [Count], Max([StartTime]) as [StartTime]
INTO [dbo].[{1}]
FROM
(
	SELECT [DatabaseName], [Error], [ApplicationName], CAST([TextData] as varchar(max)) as [ErrorText], [StartTime]
	FROM [dbo].[{0}]
	WHERE EventClass = 162
) [Errors]
GROUP BY [DatabaseName], [Error], [ApplicationName], [ErrorText]
ORDER BY [DatabaseName], [Error], [ApplicationName], [ErrorText]
            ", TableName, TableNameError);
            command.ExecuteNonQuery();
        }

        public void CreateMinuteAndSecondColumn()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
BEGIN TRANSACTION
ALTER TABLE [dbo].[{0}] ADD
	[Second01] int NULL,
	[Second05] int NULL,
	[Second10] int NULL,
	[Munute01] int NULL,
	[Munute02] int NULL,
	[Munute03] int NULL,
	[Munute04] int NULL,
	[Munute05] int NULL
ALTER TABLE [dbo].[{0}] SET (LOCK_ESCALATION = TABLE)
COMMIT
", TableName);
            command.ExecuteNonQuery();
        }

        public void FillMinuteAndSecondColumn()
        {
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
declare @minStartDate datetime
select @minStartDate = min([StartTime])
from [dbo].[{0}]
where EventClass in (10, 12)

update [dbo].[{0}]
set [Second01] = datediff(ss, @minStartDate, [StartTime]),
    [Munute01] = datediff(mi, @minStartDate, [StartTime])
where [EventClass] in (10, 12)

update [dbo].[{0}]
set [Second05] =  5 * ROUND([Second01] /  5, 0),
	[Second10] = 10 * ROUND([Second01] / 10, 0),
    [Munute02] =  2 * ROUND([Munute01] /  2, 0),
    [Munute03] =  3 * ROUND([Munute01] /  3, 0),
    [Munute04] =  4 * ROUND([Munute01] /  4, 0),
    [Munute05] =  5 * ROUND([Munute01] /  5, 0)
where [EventClass] in (10, 12)
", TableName);
            command.ExecuteNonQuery();
        }

        string GetTextData(string databaseName, string textKey, string fildName)
        {
            string result = null;
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
SELECT [{0}]
  FROM [dbo].[{1}]
 WHERE [DatabaseName] = @databaseName AND [TextKey-key] = @textKey
", fildName, TableNameDetail);
            command.Parameters.Clear();
            command.Parameters.Add(new SqlParameter("@databaseName", System.Data.SqlDbType.VarChar, 100)
            {
                Value = databaseName
            });
            command.Parameters.Add(new SqlParameter("@textKey", System.Data.SqlDbType.VarChar, 100)
            {
                Value = textKey
            });
            command.Prepare();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    result = reader.GetStringOrNull(0);
                }
            }
            reader.Close();
            TrySaveText(databaseName, textKey, fildName, result);
            return result;
        }

        void TrySaveText(string databaseName, string textKey, string field, string text)
        {
            try
            {
                if (!System.IO.Directory.Exists(databaseName))
                {
                    System.IO.Directory.CreateDirectory(databaseName);
                }
                string safeTextKey = textKey;
                foreach (char ch in System.IO.Path.GetInvalidPathChars())
                {
                    safeTextKey = safeTextKey.Replace(ch, '_');
                }
                char[] expectedChars = { '\\', '/', ':', '*', '?', '<', '>', '|' };
                foreach (char ch in expectedChars)
                {
                    safeTextKey = safeTextKey.Replace(ch, '_');
                }

                string path = databaseName + "\\" + safeTextKey;
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                System.IO.File.WriteAllText(databaseName + "\\" + safeTextKey + "\\" + field + ".sql", text);
            }
            catch
            {
            }
        }

        public Model.DetailStat FillDetailStat(Model.DetailStat node)
        {
            node.TextDataMinDuration = node.TextDataMinDuration ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-min(Duration)");
            node.TextDataMaxDuration = node.TextDataMaxDuration ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-max(Duration)");
            node.TextDataMinReads = node.TextDataMinReads ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-min(Reads)");
            node.TextDataMaxReads = node.TextDataMaxReads ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-max(Reads)");
            node.TextDataMinCPU = node.TextDataMinCPU ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-min(CPU)");
            node.TextDataMaxCPU = node.TextDataMaxCPU ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-max(CPU)");
            node.TextDataMinWrites = node.TextDataMinWrites ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-min(Writes)");
            node.TextDataMaxWrites = node.TextDataMaxWrites ?? GetTextData(node.DatabaseName, node.TextKeyKey, "TextData-max(Writes)");
            return node;
        }

        public ReadOnlyCollection<Model.DetailStat> GetDetailStat()
        {
            ReadOnlyCollection<Model.DetailStat> result = null;

            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 60 * 60;
            command.CommandText = string.Format(@"
SELECT [DatabaseName]
      ,[TextKey-key]
      ,[avg(CPU)-key]
      ,[avg(Duration)-key]
      ,[% Duration-key]
      ,[avg(Reads)-key]
      ,[Count-key]
      ,[TextKey]
      ,[min(CPU)]
      ,[avg(CPU)]
      ,[max(CPU)]
      ,[sum(CPU)]
      ,[% CPU]
      ,[min(Duration)]
      ,[avg(Duration)]
      ,[max(Duration)]
      ,[sum(Duration)]
      ,[% Duration]
      ,[min(Reads)]
      ,[avg(Reads)]
      ,[max(Reads)]
      ,[sum(Reads)]
      ,[% Reads]
      ,[min(Writes)]
      ,[avg(Writes)]
      ,[max(Writes)]
      ,[sum(Writes)]
      ,[% Writes]
      ,[Count]
      ,[% Count]
      --,[TextData-min(Duration)]
      --,[TextData-max(Duration)]
      --,[TextData-min(Reads)]
      --,[TextData-max(Reads)]
      --,[TextData-min(CPU)]
      --,[TextData-max(CPU)]
      --,[TextData-min(Writes)]
      --,[TextData-max(Writes)]
      ,[min(Duration)raw]
      ,[max(Duration)raw]
  FROM [dbo].[{0}]
", TableNameDetail);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                List<Model.DetailStat> listData = new List<Model.DetailStat>();
                while (reader.Read())
                {
                    Model.DetailStat data = new Model.DetailStat();
                    int index = 0;
                    data.DatabaseName = reader.GetStringOrNull(index); index++;
                    data.TextKeyKey = reader.GetStringOrNull(index); index++;
                    data.AvgCPUKey = reader.GetIntOrNull(index); index++;
                    data.AvgDurationKey = reader.GetLongOrNull(index); index++;
                    data.PercentDurationKey = reader.GetDoubleOrNull(index); index++;
                    data.AvgReadsKey = reader.GetLongOrNull(index); index++;
                    data.CountKey = reader.GetIntOrNull(index); index++;
                    data.TextKey = reader.GetStringOrNull(index); index++;

                    data.MinCPU = reader.GetIntOrNull(index); index++;
                    data.AvgCPU = reader.GetIntOrNull(index); index++;
                    data.MaxCPU = reader.GetIntOrNull(index); index++;
                    data.SumCPU = reader.GetIntOrNull(index); index++;
                    data.PercentCPU = reader.GetDoubleOrNull(index); index++;

                    data.MinDuration = reader.GetLongOrNull(index); index++;
                    data.AvgDuration = reader.GetLongOrNull(index); index++;
                    data.MaxDuration = reader.GetLongOrNull(index); index++;
                    data.SumDuration = reader.GetLongOrNull(index); index++;
                    data.PercentDuration = reader.GetDoubleOrNull(index); index++;

                    data.MinReads = reader.GetLongOrNull(index); index++;
                    data.AvgReads = reader.GetLongOrNull(index); index++;
                    data.MaxReads = reader.GetLongOrNull(index); index++;
                    data.SumReads = reader.GetLongOrNull(index); index++;
                    data.PercentReads = reader.GetDoubleOrNull(index); index++;

                    data.MinWrites = reader.GetLongOrNull(index); index++;
                    data.AvgWrites = reader.GetLongOrNull(index); index++;
                    data.MaxWrites = reader.GetLongOrNull(index); index++;
                    data.SumWrites = reader.GetLongOrNull(index); index++;
                    data.PercentWrites = reader.GetDoubleOrNull(index); index++;

                    data.Count = reader.GetIntOrNull(index); index++;
                    data.PercentCount = reader.GetDoubleOrNull(index); index++;

                    data.TextDataMinDuration = null; // reader.GetStringOrNull(index); index++;
                    data.TextDataMaxDuration = null; // reader.GetStringOrNull(index); index++;

                    data.TextDataMinReads = null; // reader.GetStringOrNull(index); index++;
                    data.TextDataMaxReads = null; // reader.GetStringOrNull(index); index++;

                    data.TextDataMinCPU = null; // reader.GetStringOrNull(index); index++;
                    data.TextDataMaxCPU = null; // reader.GetStringOrNull(index); index++;

                    data.TextDataMinWrites = null; // reader.GetStringOrNull(index); index++;
                    data.TextDataMaxWrites = null; // reader.GetStringOrNull(index); index++;

                    data.MinDurationRaw = reader.GetLongOrNull(index); index++;
                    data.MaxDurationRaw = reader.GetLongOrNull(index); index++;

                    listData.Add(data);
                }
                result = listData.AsReadOnly();
            }
            reader.Close();
            return result;
        }
    }
}
