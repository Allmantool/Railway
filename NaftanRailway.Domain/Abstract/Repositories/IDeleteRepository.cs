namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    public interface IDeleteRepository<T>
    {
        void Delete(T entity, bool enableDetectChanges = true);

        void Delete(Expression<Func<T, bool>> predicate, bool enableDetectChanges = true);

        void Delete(IEnumerable<T> entityColl, bool enableDetectChanges = true);

        void Delete<TK>(TK key, bool enableDetectChanges = true);
    }
}