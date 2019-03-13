using System.Linq;

namespace Railway.Domain.Interfaces.Repositories
{
    public interface IAddRepository<in T>
        where T : class, new()
    {
        void Add(T entity, bool enableDetectChanges = true);

        void Add(IQueryable<T> entityColl, bool enableDetectChanges = true);
    }
}