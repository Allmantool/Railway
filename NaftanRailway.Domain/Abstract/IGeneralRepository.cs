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
        /// <param name="enableTracking">Tracking or not change of entity</param>
        /// <param name="enableAutoDetectChanges">Enable caching on side EF</param>
        /// <returns></returns>
        IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null, bool enableTracking = true, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Get single entity 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="enableAutoDetectChanges"></param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Add general entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableAutoDetectChanges">Snapshot change detection takes a copy of every entity in the system when they are added to the Entity Framework tracking graph. Then as entities change each entity is compared to its snapshot to see any changes. This occurs by calling the enableAutoDetectChanges method. Whats important to know about enableAutoDetectChanges is that it has to go through all of your tracked entities each time its called, so the more stuff you have in your context the longer it takes to traverse.</param>
        void Add(T entity,bool enableAutoDetectChanges = true);
        /// <summary>
        /// Add Range of entity
        /// </summary>
        /// <param name="entityColl"></param>
        /// <param name="enableAutoDetectChanges"></param>
        void AddRange(IEnumerable<T> entityColl , bool enableAutoDetectChanges = true);
        /// <summary>
        /// Edit concrete entity (first attach method and then update needed property / apossite update => update all property)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableAutoDetectChanges">Snapshot change detection takes a copy of every entity in the system when they are added to the Entity Framework tracking graph. Then as entities change each entity is compared to its snapshot to see any changes. This occurs by calling the enableAutoDetectChanges method. Whats important to know about enableAutoDetectChanges is that it has to go through all of your tracked entities each time its called, so the more stuff you have in your context the longer it takes to traverse.</param>
        void Edit(T entity, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Update concrete entity  (Update all property, mark entity as modified)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="enableAutoDetectChanges">Snapshot change detection takes a copy of every entity in the system when they are added to the Entity Framework tracking graph. Then as entities change each entity is compared to its snapshot to see any changes. This occurs by calling the enableAutoDetectChanges method. Whats important to know about enableAutoDetectChanges is that it has to go through all of your tracked entities each time its called, so the more stuff you have in your context the longer it takes to traverse.</param>
        void Update(T entity, Expression<Func<T, bool>> predicate, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Update on predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableAutoDetectChanges"></param>
        void Update(T entity, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Delete concrete entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// Delete method with predicate condition
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="enableAutoDetectChanges"></param>
        void Delete(Expression<Func<T, bool>> predicate, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Update if exist or add
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableAutoDetectChanges"></param>
        void Merge(T entity, bool enableAutoDetectChanges = true);
        void Merge(IEnumerable<T> entityColl, bool enableAutoDetectChanges = true);
        /// <summary>
        /// Merge on predicate
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="excludeFieds"></param>
        /// <param name="enableAutoDetectChanges"></param>
        void Merge(T entity, Expression<Func<T, bool>> predicate, IEnumerable<string> excludeFieds, bool enableAutoDetectChanges = true);
    }
}