using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Railway.Core.Data.Interfaces.Repositories
{
    public interface IRepository<T> :
        IAsyncRepository<T>,
        IGetRepository<T>,
        IAddRepository<T>,
        IDeleteRepository<T>,
        IUpdateRepository<T>,
        IMergeRepository<T>
     where T : class
    {
        bool Exists(object primaryKey);
        int ExecuteSql(string sql, IReadOnlyCollection<SqlParameter> sqlParameters = null);
        Task<int> ExecuteSqlAsync(string sql, IReadOnlyCollection<SqlParameter> sqlParameters = null);
        IEnumerable<TEntity> SqlQuery<TEntity>(string sql, IReadOnlyCollection<SqlParameter> sqlParameters = null);

        //TODO: MOve to more appropriate class
        string GetCurrentConnection();
    }
}