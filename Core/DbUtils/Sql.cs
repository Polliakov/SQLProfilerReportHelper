using System;
using System.Data;
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

        public Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
            => ExecuteAsync(c => c.ExecuteNonQueryAsync(), query, parameters);

        public Task<int> ExecuteNonQueryAsync(int timeout, string query, params SqlParameter[] parameters)
            => ExecuteAsync(timeout, c => c.ExecuteNonQueryAsync(), query, parameters);

        public async Task<T> ExecuteScalarAsync<T>(string query, params SqlParameter[] parameters)
            => (T)await ExecuteAsync(c => c.ExecuteScalarAsync(), query, parameters);

        public async Task<T> ExecuteScalarAsync<T>(int timeout, string query, params SqlParameter[] parameters)
            => (T)await ExecuteAsync(timeout, c => c.ExecuteScalarAsync(), query, parameters);

        [Obsolete]
        public Task<SqlDataReader> ExecuteReaderAsync(string query, params SqlParameter[] parameters)
            => throw new NotImplementedException();

        [Obsolete]
        public Task<SqlDataReader> ExecuteReaderAsync(int timeout, string query, params SqlParameter[] parameters)
            => throw new NotImplementedException();

        public Task<DataSet> QueryDataSetAsync(string query, params SqlParameter[] parameters)
            => ExecuteAsync(LoadDataSet, query, parameters);

        public Task<DataSet> QueryDataSetAsync(int timeout, string query, params SqlParameter[] parameters)
            => ExecuteAsync(timeout, LoadDataSet, query, parameters);


        private Task<T> ExecuteAsync<T>(
            Func<SqlCommand, Task<T>> execute,
            string query,
            params SqlParameter[] parameters)
            => ExecuteAsync(_timeoutSeconds, execute, query, parameters);

        private async Task<T> ExecuteAsync<T>(
            int timeout,
            Func<SqlCommand, Task<T>> execute,
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
                if (parameters != null && parameters.Length > 0)
                    command.Parameters.AddRange(parameters);
                return await execute(command);
            }
        }

        private Task<DataSet> LoadDataSet(SqlCommand command)
        {
            var ds = new DataSet();
            new SqlDataAdapter(command).Fill(ds);
            return Task.FromResult(ds);
        }
    }
}
