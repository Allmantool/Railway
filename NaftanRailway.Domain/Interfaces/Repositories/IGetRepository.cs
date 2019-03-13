using System;
using System.Linq;
using System.Linq.Expressions;

namespace Railway.Domain.Interfaces.Repositories
{
    public interface IGetRepository<T>
        where T : class, new()
    {
        IQueryable<T> GetAll(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true);

        T Get(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true);

        T Find<TK>(TK key, bool enableDetectChanges = true);
    }
}