namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IDeleteRepository<T>
        where T : class, new()
    {
        void Delete(T entity, bool enableDetectChanges = true);

        void Delete(Expression<Func<T, bool>> predicate, bool enableDetectChanges = true);

        void Delete(IQueryable<T> entityColl, bool enableDetectChanges = true);

        void Delete<TK>(TK key, bool enableDetectChanges = true);
    }
}