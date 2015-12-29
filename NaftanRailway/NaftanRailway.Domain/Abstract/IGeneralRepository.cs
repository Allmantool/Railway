using System;
using System.Linq;
using System.Linq.Expressions;

namespace NaftanRailway.Domain.Abstract {
    public interface IGeneralRepository<T> {
        /// <summary>
        /// Get all or filter result
        /// </summary>
        /// <param name="predicate">Func = IEnumarable, Expression = IQueryable</param>
        /// <returns></returns>
        IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null);

        T Get(Expression<Func<T, bool>> predicate);
        
        void Add(T entity);
        
        void Edit(T entity);
        
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
    }
}
