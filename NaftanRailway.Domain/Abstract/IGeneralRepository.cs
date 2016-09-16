using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NaftanRailway.Domain.Abstract {
    public interface IGeneralRepository<T> : IDisposable {
        /// <summary>
        /// Actual working dbContext
        /// </summary>
        System.Data.Entity.DbContext Context { get; }

        /// <summary>
        /// Get all or filter result
        /// </summary>
        /// <param name="predicate">Func = IEnumarable, Expression = IQueryable</param>
        /// <param name="enableDetectChanges"></param>
        /// <param name="enableTracking"></param>
        /// <returns></returns>
        IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true);
        /// <summary>
        /// Get single entity 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="enableDetectChanges"></param>
        /// <param name="enableTracking"></param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true);
        T Find<TK>(TK key, bool enableDetectChanges = true);
        /// <summary>
        /// Add general entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges">Snapshot change detection takes a copy of every entity in the system when they are added to the Entity Framework tracking graph. Then as entities change each entity is compared to its snapshot to see any changes. This occurs by calling the DetectChanges method. Whats important to know about DetectChanges is that it has to go through all of your tracked entities each time its called, so the more stuff you have in your context the longer it takes to traverse.</param>
        void Add(T entity, bool enableDetectChanges = true);
        /// <summary>
        /// Add Range of entity
        /// </summary>
        /// <param name="entityColl"></param>
        /// <param name="enableDetectChanges"></param>
        void Add(IEnumerable<T> entityColl, bool enableDetectChanges = true);
        /// <summary>
        /// Edit concrete entity (first attach method and then update needed property / apossite update => update all property)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges"></param>
        void Edit(T entity, bool enableDetectChanges = true);
        void Edit(IEnumerable<T> entityColl, Action<T> operations, bool enableDetectChanges = true);
        /// <summary>
        /// Update concrete entity  (Update all property, mark entity as modified)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="enableDetectChanges">Snapshot change detection takes a copy of every entity in the system when they are added to the Entity Framework tracking graph. Then as entities change each entity is compared to its snapshot to see any changes. This occurs by calling the DetectChanges method. Whats important to know about DetectChanges is that it has to go through all of your tracked entities each time its called, so the more stuff you have in your context the longer it takes to traverse.</param>
        void Update(T entity, Expression<Func<T, bool>> predicate, bool enableDetectChanges = true);
        /// <summary>
        /// Update on predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges"></param>
        void Update(T entity, bool enableDetectChanges = true);
        /// <summary>
        /// Delete concrete entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges"></param>
        void Delete(T entity, bool enableDetectChanges = true);
        /// <summary>
        /// Delete method with predicate condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="enableDetectChanges"></param>
        void Delete(Expression<Func<T, bool>> predicate, bool enableDetectChanges = true);
        void Delete(IEnumerable<T> entityColl, bool enableDetectChanges = true);
        void Delete<TK>(TK key, bool enableDetectChanges = true);
        /// <summary>
        /// Update if exist or add
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges"></param>
        void Merge(T entity, bool enableDetectChanges = true);
        void Merge(IEnumerable<T> entityColl, bool enableDetectChanges = true);
        /// <summary>
        /// Merge on predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="excludeFieds"></param>
        /// <param name="enableDetectChanges"></param>
        void Merge(T entity, Expression<Func<T, bool>> predicate, IEnumerable<string> excludeFieds, bool enableDetectChanges = true);
    }
}