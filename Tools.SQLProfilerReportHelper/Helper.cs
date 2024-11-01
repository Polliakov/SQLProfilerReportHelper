namespace Tools.SQLProfilerReportHelper
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class Helper
    {
        private SqlConnection Connection { get; set; }

        string _tableName;
        /// <summary>
        /// get/set Имя таблицы с данными профайлинга
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set
            {
                if (IsTableExist(value))
                {
                    _tableName = value;
                }
                else
                {
                    throw new Exception(string.Format("Table {0} not exist", _tableName));
                }
            }
        }

        /// <summary>
        /// get Имя таблицы с отчётом по производительности всех SQL-запросов
        /// </summary>
        public string TableNameDetail
        {
            get { return TableName + ".Grouped"; }
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

        public int RowCountPrepared { get; set; }

        public void Connect(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();

            RowCountPrepared = 0;
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
ON [dbo].[{0}] ([DatabaseName],[TextKey],[ObjectName])
", TableName);
            command.ExecuteNonQuery();
        }

        public string[] GetTables()
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

        [Obsolete]
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
    ,[GroupID])
SELECT [EventClass]
      ,[LoginName]
      ,[SPID]
      ,[StartTime]
      ,[TextData]
      ,[TransactionID]
      ,DATALENGTH ( [TextData] ) as [GroupID]
FROM [dbo].[{0}]
WHERE [EventClass] = 148
ORDER BY [GroupID]
", TableName, TableNameDeadlock);
            command.ExecuteNonQuery();
        }

        public void CreateDetailReport()
        {
            try
            {
                CreateIndexOnTextKeys();
            }
            catch { }

            var scale = 4;
            var command = new SqlCommand();
            command.Connection = Connection;
            command.CommandTimeout = 160 * 60;
            command.CommandText = $@"
declare @CPUSum int; 
declare @DurationSum float; 
declare @ReadsSum float;
declare @WritesSum float;
declare @CountSum float;

select @CPUSum = SUM(CPU)
     , @DurationSum = SUM(Duration)
     , @ReadsSum = SUM(Reads)
     , @WritesSum = SUM(Writes)
     , @CountSum = count(*)
from [dbo].[{TableName}] where EventClass in (10, 12)


select
	  [DatabaseName]
	, [TextKey]
    , [ObjectName]

	, [% CPU]
	, [avg(CPU)]
	, [min(CPU)]
	, [max(CPU)]
	, [sum(CPU)]

	, [% Duration]
	, [avg(Duration)]
	, [min(Duration)]
	, [max(Duration)]
	, [sum(Duration)]

	, [% Reads]
	, [avg(Reads)]
	, [min(Reads)]
	, [max(Reads)]
	, [sum(Reads)]

	, [% Writes]
	, [avg(Writes)]
	, [min(Writes)]
	, [max(Writes)]
	, [sum(Writes)]

	, [% Count]
	, [Count]

	--, [TextData-min(Duration)]
	--, [TextData-max(Duration)]
	--, [TextData-min(Reads)]
	--, [TextData-max(Reads)]
	--, [TextData-min(CPU)]
	--, [TextData-max(CPU)]
	--, [TextData-min(Writes)]
	--, [TextData-max(Writes)]

	, [min(Duration)raw]
	, [max(Duration)raw]
    , [avg(Duration)raw]

INTO [dbo].[{TableNameDetail}]
from
(
	select 
          *
		, round(cast([sum(CPU)] as float) / @CPUSum * 100, {scale}) as [% CPU]

		, [min(Duration)raw]/1000 as [min(Duration)]
		, [avg(Duration)raw]/1000 as [avg(Duration)]
		, [max(Duration)raw]/1000 as [max(Duration)] 
		, [sum(Duration)raw]/1000 as [sum(Duration)]

		, case 
            when @DurationSum > 0 
            then round(cast([sum(Duration)raw] as float) / @DurationSum * 100, {scale})
            else 0
          end as [% Duration]

		, case
            when @ReadsSum > 0
            then round(cast([sum(Reads)] as float) / @ReadsSum * 100, {scale})
            else 0
          end as [% Reads]

		, case
            when @WritesSum > 0
            then round(cast([sum(Writes)] as float) / @WritesSum * 100, {scale})
            else 0
          end as [% Writes]

		, case
            when @CountSum > 0
            then round([Count] / @CountSum * 100, {scale})
            else 0
          end as [% Count]

		--, (select top 1 [TextData] from [{TableName}]) as [TextData-min(Duration)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-max(Duration)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-min(CPU)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-max(CPU)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-min(Reads)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-max(Reads)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-min(Writes)]
		--, (select top 1 [TextData] from [{TableName}]) as [TextData-max(Writes)]

	from
	(
		select
			[DatabaseName],
			[TextKey],
            [ObjectName],
  
			avg(CPU) as [avg(CPU)], 
			min(CPU) as [min(CPU)], 
			max(CPU) as [max(CPU)], 
			sum(CPU) as [sum(CPU)], 

			avg(Duration) as [avg(Duration)raw], 
			min(Duration) as [min(Duration)raw], 
			max(Duration) as [max(Duration)raw], 
			sum(Duration) as [sum(Duration)raw],

			avg(Reads) as [avg(Reads)],
			min(Reads) as [min(Reads)], 
			max(Reads) as [max(Reads)], 
			sum(Reads) as [sum(Reads)], 

			avg(Writes) as [avg(Writes)],
			min(Writes) as [min(Writes)], 
			max(Writes) as [max(Writes)], 
			sum(Writes) as [sum(Writes)], 

			count(*) as [Count]
		from
			[dbo].[{TableName}] as TTT -- Таблица, в которую сохранили трейс. 
		where
			EventClass in (10, 12)
		group by
			[DatabaseName], [TextKey], [ObjectName]
	) as [Statistic]
) as [Statistic2]";
            command.ExecuteNonQuery();

            command.CommandText = string.Format(@"
CREATE NONCLUSTERED INDEX [IX_TraceTableDetailStat_TextKey_DatabaseName]
ON [dbo].[{0}] ([DatabaseName],[TextKey], [ObjectName])
", TableNameDetail);
            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-min(Duration)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Duration = [dbo].[{1}].[min(Duration)raw])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-max(Duration)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Duration = [dbo].[{1}].[max(Duration)raw])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-min(CPU)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and CPU = [dbo].[{1}].[min(CPU)])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-max(CPU)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and CPU = [dbo].[{1}].[max(CPU)])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-min(Reads)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Reads = [dbo].[{1}].[min(Reads)])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-max(Reads)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Reads = [dbo].[{1}].[max(Reads)])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-min(Writes)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Writes = [dbo].[{1}].[min(Writes)])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            //            command.CommandText = string.Format(@"
            //UPDATE [dbo].[{1}] SET [TextData-max(Writes)] = 
            //(select top 1 [TextData] from [{0}] where [TextKey] = [dbo].[{1}].[TextKey] and [DatabaseName] = [dbo].[{1}].[DatabaseName] and Writes = [dbo].[{1}].[max(Writes)])
            //    ", TableName, TableNameDetail);
            //            command.ExecuteNonQuery();

            try
            {
                DropIndexOnTextKeys();
            }
            catch { }
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
    }
}
