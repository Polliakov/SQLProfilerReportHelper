using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Tools.SQLProfilerReportHelper.Database.Common
{
    public class SqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void CheckConnect(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public async Task<SqlConnection> Create()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
