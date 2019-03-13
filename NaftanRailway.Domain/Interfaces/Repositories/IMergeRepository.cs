using System;
using System.Linq;
using System.Linq.Expressions;

namespace Railway.Domain.Interfaces.Repositories
{
    public interface IMergeRepository<T>
    {
        void Merge(T entity, bool enableDetectChanges = true);

        void Merge(IQueryable<T> entityColl, bool enableDetectChanges = true);

        void Merge(
            T entity,
            Expression<Func<T, bool>> predicate,
            IQueryable<string> excludeFieds,
            bool enableDetectChanges = true);
    }
}