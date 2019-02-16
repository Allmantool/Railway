namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IAsyncRepository<T>
    {
        Task<T> GetAsync(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true);

        Task<T> FindAsync<TK>(TK key, bool enableDetectChanges = true);
    }
}