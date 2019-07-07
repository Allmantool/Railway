namespace Railway.DeliveryCargo.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Railway.DeliveryCargo.Data.EF.Domain;
    using Railway.DeliveryCargo.Data.EF.Interfaces;

    public class UnitOfWork : IUnitOfWork
    {
        private const int DefaultSqlCommandTimeoutSeconds = 200;

        private readonly string _connectionString;

        public UnitOfWork(DbContext context, string connectionString)
        {
            _connectionString = connectionString;
            Context = context;
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            Context.ChangeTracker.LazyLoadingEnabled = false;
            Context.Database.SetCommandTimeout(DefaultSqlCommandTimeoutSeconds);
        }

        protected DbContext Context { get; }

        private bool Disposed { get; set; }

        public int SaveChanges()
        {
            Context.ChangeTracker.DetectChanges();
            return Context.SaveChanges();
        }

        public Task SaveChangesAsync()
        {
            Context.ChangeTracker.DetectChanges();

            return Context.SaveChangesAsync();
        }

        public IRepository<T> Repository<T>()
            where T : class
        {
            return new Repository<T>(Context);
        }

        public IReadOnlyCollection<T> ExecuteProcedure<T>(string procedureName, SqlParameter[] parameters)
            where T : class
        {
            var commandTextWithParams = BuildProcedureQuerySql(procedureName, parameters);

            return Context.Query<T>().FromSql(commandTextWithParams, parameters).ToList();
        }

        public async Task<IReadOnlyCollection<T>> ExecuteProcedureAsync<T>(string procedureName, SqlParameter[] parameters)
            where T : class
        {
            var commandTextWithParams = BuildProcedureQuerySql(procedureName, parameters);

            return await Context.Query<T>().FromSql(commandTextWithParams, parameters).ToListAsync();
        }

        public IEnumerable<T> ExecuteDatabaseSqlQuery<T>(string sqlText, SqlParameter[] parameters)
            where T : class
        {
            return Context.Query<T>().FromSql(sqlText, parameters).ToList();
        }

        public int ExecuteNonQuerySql(string sqlText, SqlParameter[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(sqlText, parameters);
        }

        public int ExecuteNonQueryProcedure(string procedureName, SqlParameter[] parameters)
        {
            var commandTextWithParams = BuildProcedureQuerySql(procedureName, parameters);

            return Context.Database.ExecuteSqlCommand(commandTextWithParams, parameters);
        }

        public T ExecuteScalar<T>(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.CommandTimeout = DefaultSqlCommandTimeoutSeconds;
                    command.Parameters.AddRange(parameters);

                    var result = command.ExecuteScalar();

                    return result == DBNull.Value
                        ? default
                        : (T)result;
                }
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.CommandTimeout = DefaultSqlCommandTimeoutSeconds;
                    command.Parameters.AddRange(parameters);

                    var result = await command.ExecuteScalarAsync();

                    return result == DBNull.Value
                        ? default
                        : (T)result;
                }
            }
        }

        public DbDataReader ExecuteSqlReader(string sql, IEnumerable<DatabaseParameter> parameters = null)
        {
            var connection = new SqlConnection(_connectionString);

            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.CommandTimeout = DefaultSqlCommandTimeoutSeconds;
                    AddParameters(command, parameters);

                    connection.Open();

                    return command.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch
            {
                connection.Close();
                throw;
            }
        }

        public IEnumerable<SqlParameter> ExecuteNonQuery(string sqlText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandType = commandType;
                    command.CommandText = sqlText;
                    command.CommandTimeout = DefaultSqlCommandTimeoutSeconds;
                    command.Parameters.AddRange(parameters);
                    command.ExecuteNonQuery();

                    var retparms = command.Parameters.Cast<SqlParameter>()
                        .Where(sqlParameter => sqlParameter.Direction == ParameterDirection.Output)
                        .ToList();

                    return retparms;
                }
            }
        }

        public DataSet ExecuteSql(string sql, IEnumerable<DatabaseParameter> parameters = null)
        {
            var resultSet = new DataSet();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandType = CommandType.Text;
                    command.CommandText = sql;
                    command.CommandTimeout = DefaultSqlCommandTimeoutSeconds;
                    AddParameters(command, parameters);

                    // Create the DbDataAdapter.
                    var adapter = CreateDataAdapter(connection);
                    adapter.SelectCommand = command;
                    adapter.Fill(resultSet);
                }
            }

            return resultSet;
        }

        public Task<List<T>> ToListAsync<T>(IQueryable<T> sourceQuery)
        {
            return sourceQuery.ToListAsync();
        }

        public Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TSource, TKey, TValue>(
            IQueryable<TSource> sourceQuery,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector)
        {
            return sourceQuery.ToDictionaryAsync(keySelector, valueSelector);
        }

        public Task<T> FirstAsync<T>(IQueryable<T> sourceQuery)
        {
            return sourceQuery.FirstAsync();
        }

        public Task<T> FistOrDefaultAsync<T>(IQueryable<T> sourceQuery)
        {
            return sourceQuery.FirstOrDefaultAsync();
        }

        public Task<T> FistOrDefaultAsync<T>(IQueryable<T> sourceQuery, Expression<Func<T, bool>> predicate)
        {
            return sourceQuery.FirstOrDefaultAsync(predicate);
        }

        public Task<int> CountAsync<T>(IQueryable<T> sourceQuery, Expression<Func<T, bool>> predicate)
        {
            return sourceQuery.CountAsync(predicate);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (Context != null && disposing)
                {
                    Context.Dispose();
                }
            }

            Disposed = true;
        }

        private string BuildProcedureQuerySql(string procedureName, SqlParameter[] parameters)
        {
            var parameterString = string.Join(", ", parameters.Select(FormatParameterName));

            return procedureName + (!string.IsNullOrEmpty(parameterString) ? " " + parameterString : parameterString);
        }

        private string FormatParameterName(SqlParameter parameter)
        {
            var formattedName = parameter.ParameterName;
            if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
            {
                formattedName += " out";
            }

            return formattedName;
        }

        private void AddParameters(DbCommand command, IEnumerable<DatabaseParameter> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var parameter in parameters)
            {
                var newParameter = command.CreateParameter();
                newParameter.ParameterName = parameter.ParameterName;
                newParameter.Value = parameter.Value;

                if (parameter.Type.HasValue)
                {
                    newParameter.DbType = parameter.Type.Value;
                }

                if (parameter.DataSize.HasValue)
                {
                    newParameter.Size = parameter.DataSize.Value;
                }

                command.Parameters.Add(newParameter);
            }
        }

        private DbDataAdapter CreateDataAdapter(DbConnection connection)
        {
            if (connection is SqlConnection)
            {
                return new SqlDataAdapter();
            }

            return new SqlDataAdapter();
        }
    }
}
