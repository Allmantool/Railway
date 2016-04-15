using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using NaftanRailway.Domain.Abstract;

namespace NaftanRailway.Domain.Concrete {
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class {
        private bool _disposed;
        public System.Data.Entity.DbContext _context { get; private set; }
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(System.Data.Entity.DbContext context) {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null) {
            if (predicate != null) {
                /*//sync data in Db & EF (if change not tracking for EF)
                ((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
                _context.Entry(_dbSet.Where(predicate)).Reload(); EF 4.1+
                _context.SaveChanges();*/
                
                return _dbSet.Where(predicate);
            }

            //sync data in Db & EF (if change not tracking for EF)
            //((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet);
            // _context.Entry(_dbSet.GetType()).Reload();
            return _dbSet;
        }

        public T Get(Expression<Func<T, bool>> predicate = null) {
            //sync data in Db & EF (if change not tracking for EF)
            //var ctx = ((IObjectContextAdapter) _context).ObjectContext;
            //ctx.Refresh(RefreshMode.StoreWins, ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified));
            //((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
            //_context.Entry(_dbSet.Where(predicate)).Reload();
            //_context.SaveChanges();

            return predicate == null ? _dbSet.FirstOrDefault() : _dbSet.FirstOrDefault(predicate);
        }

        public void Add(T entity) {
            _dbSet.Add(entity);
        }
        /// <summary>
        /// Mark all field of record as dirty => update all field (marking the whole entity as dirty)
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity) {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Expression<Func<T, bool>> predicate) {
            var entity = _dbSet.FirstOrDefault(predicate);

            if (entity != null)
                _context.Entry(entity).State = EntityState.Deleted;
        }

        public void Delete(T entity) {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// if call saveChanges after change some property, this EntityState update only need field
        /// </summary>
        /// <param name="entity"></param>
        public void Edit(T entity) {
            //_dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Unchanged;
        }

        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing)
                    _context.Dispose();
            }
            _disposed = true;
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}