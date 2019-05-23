namespace Railway.Core.Data.Interfaces.Repositories
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IAsyncRepository<T>
        where T : class
    {
        Task<T> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true);

        Task<T> FindAsync<TK>(TK key, bool enableDetectChanges = true);
    }
}