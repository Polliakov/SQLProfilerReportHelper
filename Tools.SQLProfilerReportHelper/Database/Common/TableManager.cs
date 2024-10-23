using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tools.SQLProfilerReportHelper.Database.Common
{
    internal class TableManager
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly Sql _sql;

        public TableManager(SqlConnectionFactory connectionFactory, Sql sql)
        {
            _connectionFactory = connectionFactory;
            _sql = sql;
        }

        public async Task<bool> IsTableExist(string tableName)
        {
            var count = (int)await _sql.ExecuteScalarAsync(@"
select count(*)
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @tableName
order by TABLE_NAME",
                new SqlParameter("@tableName", tableName));
            return count > 0;
        }

        public async Task<bool> ColumnExistInTable(string tableName, string columnName = "TextKey")
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
    }
}
