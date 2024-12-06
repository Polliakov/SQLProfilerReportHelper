using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TraceKnife.Common
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Is null or white space.", nameof(connectionString));

            _connectionString = connectionString;
        }

        public async Task<SqlConnection> CreateAsync()
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public SqlConnection Create()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
