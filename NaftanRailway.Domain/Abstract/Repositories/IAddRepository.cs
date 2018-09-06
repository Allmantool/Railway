namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System.Linq;

    public interface IAddRepository<T>
    {
        /// <summary>
        /// Add general entity.
        /// </summary>
        /// <param name="entity">The instance of entity.</param>
        /// <param name="enableDetectChanges">
        /// Snapshot change detection takes a copy of every entity in the system when they are
        /// added to the Entity Framework tracking graph. Then as entities change each entity is compared to its snapshot to
        /// see any changes. This occurs by calling the DetectChanges method. What is important to know about DetectChanges is
        /// that it has to go through all of your tracked entities each time its called, so the more stuff you have in your
        /// context the longer it takes to traverse.
        /// </param>
        void Add(T entity, bool enableDetectChanges = true);

        void Add(IQueryable<T> entityColl, bool enableDetectChanges = true);
    }
}