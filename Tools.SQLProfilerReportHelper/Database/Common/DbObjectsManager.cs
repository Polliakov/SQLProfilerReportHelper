using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tools.SQLProfilerReportHelper.Database.Common
{
    public class DbObjectsManager
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly Sql _sql;

        public DbObjectsManager(SqlConnectionFactory connectionFactory, Sql sql)
        {
            _connectionFactory = connectionFactory;
            _sql = sql;
        }

        /// <summary>
        /// get Имя таблицы с отчётом по производительности хранимых процедур (быстрый черновой отчёт)
        /// </summary>
        public string GetTableNameDraft(string traceTable) => traceTable + ".DraftStat";

        /// <summary>
        /// get Имя таблицы с отчётом по производительности всех SQL-запросов
        /// </summary>
        public string GetTableNameDetail(string traceTable) => traceTable + ".DetailStat";

        /// <summary>
        /// get Имя таблицы с отчётом по статистике ошибок
        /// </summary>
        public string GetTableNameError(string traceTable) => traceTable + ".ErrorStat";

        /// <summary>
        /// get Имя таблицы с взаимоблокировками
        /// </summary>
        public string GetTableNameDeadlock(string traceTable) => traceTable + ".DeadlockGraphs";

        public async Task<bool> IsTableExist(string tableName)
        {
            var count = (int)await _sql.ExecuteScalarAsync(@"
select count(*)
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @tableName",
                new SqlParameter("@tableName", tableName));
            return count > 0;
        }

        public async Task<bool> IsColumnExistInTable(string tableName, string columnName = "TextKey")
        {
            var count = (int)await _sql.ExecuteScalarAsync(@"
select count(*)
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @tableName
and COLUMN_NAME = @columnName",
            new SqlParameter("@tableName", System.Data.SqlDbType.VarChar, 100)
            {
                Value = tableName
            },
            new SqlParameter("@columnName", System.Data.SqlDbType.VarChar, 100)
            {
                Value = columnName
            });

            return count > 0;
        }

        public async Task<bool> IsFunctionExists(string functionName)
        {
            var count = (int)await _sql.ExecuteScalarAsync(@"
select count(*)
from sys.sql_modules m 
inner join sys.objects o on m.object_id=o.object_id
where o.type = 'FN' and name=@functionName",
                new SqlParameter("@functionName", System.Data.SqlDbType.NVarChar, 128)
                {
                    Value = functionName
                });

            return count > 0;
        }
    }
}
