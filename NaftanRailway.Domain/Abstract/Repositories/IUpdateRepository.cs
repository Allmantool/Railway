namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IUpdateRepository<T>
    {
        void Update(T entity, IEnumerable<Action<T>> operations, bool enableDetectChanges = true);

        void Upate(IQueryable<T> entities, IEnumerable<Action<T>> operations, bool enableDetectChanges = true);

        void Upate(Expression<Func<T, bool>> predicate, IEnumerable<Action<T>> operations, bool enableDetectChanges = true);
    }
}