namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IMergeRepository<T>
    {
        void Merge(T entity, bool enableDetectChanges = true);

        void Merge(IEnumerable<T> entityColl, bool enableDetectChanges = true);

        void Merge(T entity, Expression<Func<T, bool>> predicate, IEnumerable<string> excludeFieds, bool enableDetectChanges = true);
    }
}