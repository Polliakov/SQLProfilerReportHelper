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

        public int RowCountPrepared { get; set; }

        public void Connect(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();

            RowCountPrepared = 0;
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
