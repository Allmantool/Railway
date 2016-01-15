using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using NaftanRailway.Domain.Abstract;

namespace NaftanRailway.Domain.Concrete {
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class {
        private readonly System.Data.Entity.DbContext _context;
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(System.Data.Entity.DbContext context) {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null) {
            if(predicate != null)
                return _dbSet.Where(predicate);

            return _dbSet;
        }

        public T Get(Expression<Func<T, bool>> predicate) {
            return _dbSet.FirstOrDefault(predicate);
        }

        public void Add(T entity) {
            _dbSet.Add(entity);
        }
        /// <summary>
        /// Mark all field of record as dirty => update all field
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity) {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Expression<Func<T, bool>> predicate) {
            var entity  = _dbSet.FirstOrDefault(predicate);

            if(entity != null)
                _context.Entry(entity).State = EntityState.Deleted;
        }

        public void Delete(T entity) {
            _context.Entry(entity).State=EntityState.Deleted;
        }

        /// <summary>
        /// if call saveChanges after change some property, this EntityState update only need field
        /// </summary>
        /// <param name="entity"></param>
        public void Edit(T entity) {
            _context.Entry(entity).State = EntityState.Unchanged;
        }
    }
}
