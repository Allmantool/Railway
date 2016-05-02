using System;
using System.Linq;
using System.Linq.Expressions;

namespace NaftanRailway.Domain.Abstract {
    public interface IGeneralRepository<T> : IDisposable {
        /// <summary>
        /// Actual working dbContext
        /// </summary>
        System.Data.Entity.DbContext _context { get; }

        /// <summary>
        /// Get all or filter result
        /// </summary>
        /// <param name="predicate">Func = IEnumarable, Expression = IQueryable</param>
        /// <param name="enablecaching">Enable caching on side EF</param>
        /// <returns></returns>
        IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null, bool enablecaching = true);
        /// <summary>
        /// Get single entity 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add general entity
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);
        /// <summary>
        /// Edit concrete entity (first attach method and then update needed property / apossite update => update all property)
        /// </summary>
        /// <param name="entity"></param>
        void Edit(T entity);
        /// <summary>
        /// Update concrete entity  (Update all property, mark entity as modified)
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary>
        /// Delete concrete entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// Delete method with predicate condition
        /// </summary>
        /// <param name="predicate"></param>
        void Delete(Expression<Func<T, bool>> predicate);
    }
}
