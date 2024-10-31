using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using TraceKnife.Core.DbUtils;

namespace TraceKnife.Common
{
    public class Sql
    {
        private readonly SqlConnectionFactory _connectionFactory;
        private readonly int _timeoutSeconds;

        public Sql(SqlConnectionFactory connectionFactory, int timeoutSeconds)
        {
            _connectionFactory = connectionFactory;
            _timeoutSeconds = timeoutSeconds;
        }

        public async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
            => (int)await ExecuteAsync(async c => await c.ExecuteNonQueryAsync(),
               query, parameters);

        public async Task<int> ExecuteNonQueryAsync(int timeout, string query, params SqlParameter[] parameters)
            => (int)await ExecuteAsync(timeout,
               async c => await c.ExecuteNonQueryAsync(),
               query, parameters);

        public async Task<T> ExecuteScalarAsync<T>(string query, params SqlParameter[] parameters)
            => (T)await ExecuteAsync(async c => await c.ExecuteScalarAsync(),
               query, parameters);

        public async Task<T> ExecuteScalarAsync<T>(int timeout, string query, params SqlParameter[] parameters)
            => (T)await ExecuteAsync(timeout,
                   async c => await c.ExecuteScalarAsync(),
                   query, parameters);

        private Task<object> ExecuteAsync(
            Func<SqlCommand, Task<object>> execute,
            string query,
            params SqlParameter[] parameters)
            => ExecuteAsync(_timeoutSeconds, execute, query, parameters);

        private async Task<object> ExecuteAsync(
            int timeout,
            Func<SqlCommand, Task<object>> execute,
            string query,
            params SqlParameter[] parameters)
        {
            using (var connection = await _connectionFactory.Create())
            {
                var command = new SqlCommand()
                {
                    Connection = connection,
                    CommandTimeout = timeout,
                    CommandText = query,
                };
                if (parameters != null)
                    command.Parameters.AddRange(parameters);
                return await execute(command);
            }
        }
    }
}
