namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System.Linq;

    public interface IAddRepository<in T>
        where T : class, new()
    {
        void Add(T entity, bool enableDetectChanges = true);

        void Add(IQueryable<T> entityColl, bool enableDetectChanges = true);
    }
}