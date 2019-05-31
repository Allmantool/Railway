namespace Railway.DeliveryCargo.Data.EF.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Railway.DeliveryCargo.Data.EF.Domain;

    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Save all changes to the underlying data store.
        /// </summary>
        /// <returns>Returns affected rows count.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously Save all changes to the underlying data store.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveChangesAsync();

        /// <summary>
        /// Retrieve a repository for the given entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>A repository.</returns>
        IRepository<T> Repository<T>()
            where T : class;

        /// <summary>
        /// Execute a stored procedure with the given parameters using plain database query.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">Parameters to supply when executing the stored procedure.</param>
        /// <typeparam name="T">The entity type returned by the stored procedure.</typeparam>
        /// <returns>A list of resulting entities.</returns>
        IReadOnlyCollection<T> ExecuteProcedure<T>(string procedureName, SqlParameter[] parameters)
            where T : class;

        /// <summary>
        /// Asynchronously execute a stored procedure with the given parameters using plain database query.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">Parameters to supply when executing the stored procedure.</param>
        /// <typeparam name="T">The entity type returned by the stored procedure.</typeparam>
        /// <returns>A task that resolves to a list of resulting entities.</returns>
        Task<IReadOnlyCollection<T>> ExecuteProcedureAsync<T>(string procedureName, SqlParameter[] parameters)
            where T : class;

        /// <summary>
        /// Execute a SQL query with the given parameters using plain database query.
        /// </summary>
        /// <param name="sqlText">The SQL text that will be execute.</param>
        /// <param name="parameters">Parameters to supply when executing the SQL query.</param>
        /// <typeparam name="T">The entity type returned by the SQL query.</typeparam>
        /// <returns>A list of resulting entities.</returns>
        IEnumerable<T> ExecuteDatabaseSqlQuery<T>(string sqlText, params SqlParameter[] parameters)
            where T : class;

        /// <summary>
        /// Executes a plain SQL statement with the given parameters and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlText">SQL text to execute.</param>
        /// <param name="parameters">Parameters to supply when executing the SQL text.</param>
        /// <returns>The number of rows affected</returns>
        int ExecuteNonQuerySql(string sqlText, SqlParameter[] parameters);

        /// <summary>
        /// Executes a plain SQL statement with the given parameters and returns parameters.
        /// </summary>
        /// <param name="sqlText">SQL text to execute.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="parameters">Parameters to supply when executing the SQL text.</param>
        /// <returns>Method return parameters with output direction</returns>
        IEnumerable<SqlParameter> ExecuteNonQuery(string sqlText, CommandType commandType, params SqlParameter[] parameters);

        /// <summary>
        /// Executes a stored procedure with the given parameters and returns the number of rows affected.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">Parameters to supply when executing the stored procedure.</param>
        /// <returns>The number of rows affected</returns>
        int ExecuteNonQueryProcedure(string procedureName, SqlParameter[] parameters);

        /// <summary>
        /// Execute a sql query against the underlying database. The resulting data reader must be disposed by the consumer,
        /// and another should not be opened before it is disposed.
        /// </summary>
        /// <param name="sql">The SQL query text.</param>
        /// <param name="parameters">A collection of parameter name-value pairs corresponding to those found in the SQL text.</param>
        /// <returns>A data reader containing the results.</returns>
        DbDataReader ExecuteSqlReader(string sql, IEnumerable<DatabaseParameter> parameters = null);

        /// <summary>
        /// Execute a sql query against the underlying database. The resulting data set must be disposed by the consumer,
        /// and another should not be opened before it is disposed.
        /// </summary>
        /// <param name="sql">The SQL query text.</param>
        /// <param name="parameters">A collection of parameter name-value pairs corresponding to those found in the SQL text.</param>
        /// <returns>A data reader containing the results.</returns>
        DataSet ExecuteSql(string sql, IEnumerable<DatabaseParameter> parameters = null);

        /// <summary>
        /// Executes a plain SQL statement with the given parameters and returns the value.
        /// </summary>
        /// <param name="sql">The SQL query text.</param>
        /// <param name="parameters">A collection of parameter name-value pairs corresponding to those found in the SQL text.</param>
        /// <typeparam name="T">The entity type returned by the SQL query.</typeparam>
        /// <returns>object</returns>
        T ExecuteScalar<T>(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// Asynchronously executes a plain SQL statement with the given parameters and returns the value.
        /// </summary>
        /// <param name="sql">The SQL query text.</param>
        /// <param name="parameters">A collection of parameter name-value pairs corresponding to those found in the SQL text.</param>
        /// <typeparam name="T">The entity type returned by the SQL query.</typeparam>
        /// <returns>object</returns>
        Task<T> ExecuteScalarAsync<T>(string sql, params SqlParameter[] parameters);

        /// <summary>
        /// Creates a List from an IQueryable by enumerating it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the item contained by the list.</typeparam>
        /// <param name="sourceQuery">The query to execute.</param>
        /// <returns>The result of the query as a list.</returns>
        Task<List<T>> ToListAsync<T>(IQueryable<T> sourceQuery);

        /// <summary>
        /// Creates a Dictionary from an IQueryable by enumerating it asynchronously
        /// according to a specified key selector and an element selector function.
        /// </summary>
        /// <typeparam name="TSource">The type of the source item.</typeparam>
        /// <typeparam name="TKey">The type of the key item.</typeparam>
        /// <typeparam name="TValue">The type of the value item.</typeparam>
        /// <param name="sourceQuery">The source queryable.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="valueSelector">The value selector.</param>
        /// <returns>The result dictionary.</returns>
        Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TSource, TKey, TValue>(
            IQueryable<TSource> sourceQuery,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector);

        /// <summary>
        /// Asynchronously returns the first element of a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="sourceQuery">The query to execute.</param>
        /// <returns>The result of the execution.</returns>
        Task<T> FirstAsync<T>(IQueryable<T> sourceQuery);

        /// <summary>
        /// Asynchronously returns the first element of a sequence, or a default value if
        /// the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="sourceQuery">The query to execute.</param>
        /// <returns>The result of the execution or the default value.</returns>
        Task<T> FistOrDefaultAsync<T>(IQueryable<T> sourceQuery);

        /// <summary>
        /// Asynchronously returns the first element of a sequence, or a default value if
        /// the sequence contains no elements. Applies a predicate.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="sourceQuery">The query to execute.</param>
        /// <param name="predicate">A predicate to apply before execution.</param>
        /// <returns>The result of the execution or the default value.</returns>
        Task<T> FistOrDefaultAsync<T>(IQueryable<T> sourceQuery, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Asynchronously returns the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <typeparam name="T">The type of the items returned by the query.</typeparam>
        /// <param name="sourceQuery">The query for which the count will be performed.</param>
        /// <param name="predicate">The predicate to apply to the query.</param>
        /// <returns>The count.</returns>
        Task<int> CountAsync<T>(IQueryable<T> sourceQuery, Expression<Func<T, bool>> predicate);
    }
}
