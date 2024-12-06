using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace TraceKnife.Common
{
    public class Sql
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly int _timeoutSeconds;

        public Sql(ISqlConnectionFactory connectionFactory, int timeoutSeconds = 60)
        {
            _connectionFactory = connectionFactory;
            _timeoutSeconds = timeoutSeconds;
        }

        public Sql(string connectionString, int timeoutSeconds = 60)
            : this(new SqlConnectionFactory(connectionString), timeoutSeconds) { }


        #region Async Methods
        public Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
            => ExecuteAsync(c => c.ExecuteNonQueryAsync(), query, parameters);

        public Task<int> ExecuteNonQueryAsync(SqlOptions options, string query, params SqlParameter[] parameters)
            => ExecuteAsync(options, c => c.ExecuteNonQueryAsync(), query, parameters);

        public async Task<T> ExecuteScalarAsync<T>(string query, params SqlParameter[] parameters)
            => (T)await ExecuteAsync(c => c.ExecuteScalarAsync(), query, parameters);

        public async Task<T> ExecuteScalarAsync<T>(SqlOptions options, string query, params SqlParameter[] parameters)
            => (T)await ExecuteAsync(options, c => c.ExecuteScalarAsync(), query, parameters);

#if NETCOREAPP2_1_OR_GREATHER
        public Task<IEnumerable<IDataRecord>> LazyQueryAsync(string query, params SqlParameter[] parameters)
            => LazyQueryAsync(new SqlOptions(), query, parameters);

        public IAsyncEnumerable<IDataRecord> LazyQueryAsync(string query, params SqlParameter[] parameters)
            => LazyQueryAsync(new SqlOptions(), query, parameters);

        public async IAsyncEnumerable<IDataRecord> LazyQueryAsync(SqlOptions options, string query, params SqlParameter[] parameters)
        {
            using (var connection = await _connectionFactory.CreateAsync())
            {
                var command = NewCommand(options, query, parameters, connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        yield return reader;
                    }
                }
            }
        }
#endif

        public Task<DataTable> QueryAsync(string query, params SqlParameter[] parameters)
            => ExecuteAsync(LoadTableAsyncContext, query, parameters);

        public Task<DataTable> QueryAsync(SqlOptions options, string query, params SqlParameter[] parameters)
            => ExecuteAsync(options, LoadTableAsyncContext, query, parameters);

        public Task<List<DataTable>> QueryListAsync(string query, params SqlParameter[] parameters)
            => ExecuteAsync(LoadTableListAsyncContext, query, parameters);

        public Task<List<DataTable>> QueryListAsync(SqlOptions options, string query, params SqlParameter[] parameters)
            => ExecuteAsync(options, LoadTableListAsyncContext, query, parameters);

        public Task<DataSet> QueryDataSetAsync(string query, params SqlParameter[] parameters)
            => ExecuteAsync(LoadDataSetAsyncContext, query, parameters);

        public Task<DataSet> QueryDataSetAsync(SqlOptions options, string query, params SqlParameter[] parameters)
            => ExecuteAsync(options, LoadDataSetAsyncContext, query, parameters);
        #endregion

        #region Sync Methods
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
            => Execute(c => c.ExecuteNonQuery(), query, parameters);

        public int ExecuteNonQuery(SqlOptions options, string query, params SqlParameter[] parameters)
            => Execute(options, c => c.ExecuteNonQuery(), query, parameters);

        public T ExecuteScalar<T>(string query, params SqlParameter[] parameters)
            => (T)Execute(c => c.ExecuteScalar(), query, parameters);

        public T ExecuteScalar<T>(SqlOptions options, string query, params SqlParameter[] parameters)
            => (T)Execute(options, c => c.ExecuteScalar(), query, parameters);

        public IEnumerable<IDataRecord> LazyQuery(string query, params SqlParameter[] parameters)
            => LazyQuery(new SqlOptions(), query, parameters);

        public IEnumerable<IDataRecord> LazyQuery(SqlOptions options, string query, params SqlParameter[] parameters)
        {
            using (var connection = _connectionFactory.Create())
            {
                var command = NewCommand(options, query, parameters, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public DataTable Query(string query, params SqlParameter[] parameters)
            => Execute(LoadTable, query, parameters);

        public DataTable Query(SqlOptions options, string query, params SqlParameter[] parameters)
            => Execute(options, LoadTable, query, parameters);

        public List<DataTable> QueryList(string query, params SqlParameter[] parameters)
            => Execute(LoadTableList, query, parameters);

        public List<DataTable> QueryList(SqlOptions options, string query, params SqlParameter[] parameters)
            => Execute(options, LoadTableList, query, parameters);

        public DataSet QueryDataSet(string query, params SqlParameter[] parameters)
            => Execute(LoadDataSet, query, parameters);

        public DataSet QueryDataSet(SqlOptions options, string query, params SqlParameter[] parameters)
            => Execute(options, LoadDataSet, query, parameters);
        #endregion

        private Task<T> ExecuteAsync<T>(
            Func<SqlCommand, Task<T>> execute,
            string query,
            params SqlParameter[] parameters)
            => ExecuteAsync(new SqlOptions(), execute, query, parameters);

        private async Task<T> ExecuteAsync<T>(
            SqlOptions options,
            Func<SqlCommand, Task<T>> execute,
            string query,
            params SqlParameter[] parameters)
        {
            using (var connection = await _connectionFactory.CreateAsync())
            {
                SqlCommand command = NewCommand(options, query, parameters, connection);
                return await execute(command);
            }
        }

        private T Execute<T>(
            Func<SqlCommand, T> execute,
            string query,
            params SqlParameter[] parameters)
            => Execute(new SqlOptions(), execute, query, parameters);

        private T Execute<T>(
            SqlOptions options,
            Func<SqlCommand, T> execute,
            string query,
            params SqlParameter[] parameters)
        {
            using (var connection = _connectionFactory.Create())
            {
                SqlCommand command = NewCommand(options, query, parameters, connection);
                return execute(command);
            }
        }

        private SqlCommand NewCommand(
            SqlOptions options,
            string query,
            SqlParameter[] parameters,
            SqlConnection connection)
        {
            var command = new SqlCommand()
            {
                Connection = connection,
                CommandTimeout = options.Timeout > 0 ? options.Timeout : _timeoutSeconds,
                CommandText = query,
            };
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var parameter in parameters.Where(p => p.Value is null))
                    parameter.Value = DBNull.Value;
                command.Parameters.AddRange(parameters);
            }

            return command;
        }

        private static Task<DataTable> LoadTableAsyncContext(SqlCommand command)
            => Task.FromResult(LoadTable(command));

        private static DataTable LoadTable(SqlCommand command)
        {
            var dt = new DataTable();
            new SqlDataAdapter(command).Fill(dt);
            return dt;
        }

        private static Task<List<DataTable>> LoadTableListAsyncContext(SqlCommand command)
            => Task.FromResult(LoadTableList(command));

        private static List<DataTable> LoadTableList(SqlCommand command)
        {
            var ds = new DataSet();
            new SqlDataAdapter(command).Fill(ds);
            return TablesToList(ds.Tables);
        }

        private static Task<DataSet> LoadDataSetAsyncContext(SqlCommand command)
            => Task.FromResult(LoadDataSet(command));

        private static DataSet LoadDataSet(SqlCommand command)
        {
            var ds = new DataSet();
            new SqlDataAdapter(command).Fill(ds);
            return ds;
        }

        private static List<DataTable> TablesToList(DataTableCollection tables)
        {
            var list = new List<DataTable>(tables.Count);
            foreach (DataTable table in tables)
                list.Add(table);
            return list;
        }
    }
}
