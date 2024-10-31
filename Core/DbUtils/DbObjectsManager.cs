using System.Data.SqlClient;
using System.Threading.Tasks;
using TraceKnife.Common;

namespace TraceKnife.Core.DbUtils
{
    public class DbObjectsManager
    {
        private readonly Sql _sql;

        public DbObjectsManager(Sql sql)
        {
            _sql = sql;
        }

        public async Task<int> GetRowsCount(string tableName)
        {
            return await _sql.ExecuteScalarAsync<int>($"select count(*) from {tableName}");
        }

        public async Task<bool> IsTableExist(string tableName)
        {
            var count = await _sql.ExecuteScalarAsync<int>(@"
select count(*)
from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @tableName",
                new SqlParameter("@tableName", tableName));
            return count > 0;
        }

        public async Task<bool> IsColumnExistInTable(string tableName, string columnName = "TextKey")
        {
            var count = await _sql.ExecuteScalarAsync<int>(@"
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
            var count = await _sql.ExecuteScalarAsync<int>(@"
select count(*)
from sys.sql_modules m 
inner join sys.objects o on m.object_id=o.object_id
where o.type = 'FN' and name=@functionName", new SqlParameter("@functionName", System.Data.SqlDbType.NVarChar, 128) { Value = functionName });

            return count > 0;
        }
    }
}
