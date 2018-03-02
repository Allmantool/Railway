namespace NaftanRailway.Domain.Abstract
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// The GeneralRepository interface.
    /// </summary>
    /// <typeparam name="T"> Generic entity.</typeparam>
    public interface IGeneralRepository<T> : IDisposable
    {
        DbContext ActiveDbContext { get; set; }

        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true);

        T Get(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true);

        Task<T> GetAsync(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true);

        T Find<TK>(TK key, bool enableDetectChanges = true);

        Task<T> FindAsync<TK>(TK key, bool enableDetectChanges = true);

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

        void Add(IEnumerable<T> entityColl, bool enableDetectChanges = true);

        /// <summary>
        /// Edit concrete entity (first attach method and then update needed property / apposite update => update all property).
        /// </summary>
        /// <param name="entity"> Current Entity. </param>
        /// <param name="enableDetectChanges"> Detect or not entity changes.</param>
        void Edit(T entity, bool enableDetectChanges = true);

        void Edit(IEnumerable<T> entityColl, Action<T> operations, bool enableDetectChanges = true);

        void Update(T entity, Expression<Func<T, bool>> predicate, bool enableDetectChanges = true);

        void Update(T entity, bool enableDetectChanges = true);

        void Delete(T entity, bool enableDetectChanges = true);

        void Delete(Expression<Func<T, bool>> predicate, bool enableDetectChanges = true);

        void Delete(IEnumerable<T> entityColl, bool enableDetectChanges = true);

        void Delete<TK>(TK key, bool enableDetectChanges = true);

        void Merge(T entity, bool enableDetectChanges = true);

        void Merge(IEnumerable<T> entityColl, bool enableDetectChanges = true);

        void Merge(T entity, Expression<Func<T, bool>> predicate, IEnumerable<string> excludeFieds, bool enableDetectChanges = true);

        bool Exists(object primaryKey);
    }
}