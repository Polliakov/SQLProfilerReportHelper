using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace TraceKnife.Core.DbUtils
{
    public static class SqlExtensions
    {
        public static T GetValue<T>(this SqlDataReader reader, string column)
        {
            try
            {
                return (T)reader[column];
            }
            catch
            {
                return default;
            }
        }

        public static T GetValue<T>(this DataRow row, string column)
        {
            try
            {
                return (T)row[column];
            }
            catch
            {
                return default;
            }
        }

        public static bool IsEmpty(this DataSet ds)
        {
            if (ds.Tables.Count == 0)
                return true;

            return Enumerable.Range(0, ds.Tables.Count)
                .Any(i => ds.Tables[i].Rows.Count == 0);
        }

        public static DataRow First(this DataSet ds)
            => ds.Tables[0].Rows[0];
    }
}
