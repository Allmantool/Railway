using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using NaftanRailway.Domain.Abstract;

namespace NaftanRailway.Domain.Concrete {
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class {
        private bool _disposed;
        public System.Data.Entity.DbContext Context { get; private set; }
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(System.Data.Entity.DbContext context) {
            Context = context;
            _dbSet = context.Set<T>();
        }
        /// <summary>
        /// Get lazy data set (with cashing or not (attr MergeOption )
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="enablecaching"></param>
        /// <returns></returns>
        public IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null, bool enablecaching = true) {
            if (predicate != null) {
                /*//sync data in Db & EF (if change not tracking for EF)
                ((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
                _context.Entry(_dbSet.Where(predicate)).Reload(); EF 4.1+
                _context.SaveChanges();*/

                return (enablecaching) ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);
            }

            //sync data in Db & EF (if change not tracking for EF)
            //((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet);
            // _context.Entry(_dbSet.GetType()).Reload();
            return (enablecaching) ? _dbSet.AsNoTracking() : _dbSet;
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

        public void Add(T entity, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            _dbSet.Add(entity);
        }

        /// <summary>
        /// Mark all field of record as dirty => update all field (marking the whole entity as dirty)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="detectChanges"></param>
        public void Update(T entity, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Expression<Func<T, bool>> predicate) {
            var entitysRange = _dbSet.Where(predicate);

            foreach (var entity in entitysRange){
                Context.Entry(entity).State = EntityState.Deleted;
            }
                
        }

        public void Delete(T entity) {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// if call saveChanges after change some property, this EntityState update only need field
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="detectChanges"></param>
        public void Edit(T entity, bool detectChanges = true) {
            //_dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Unchanged;
        }

        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing)
                    Context.Dispose();
            }
            _disposed = true;
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}