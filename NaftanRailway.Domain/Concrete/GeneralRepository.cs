using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
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

        public T Get(Expression<Func<T, bool>> predicate = null, bool enablecaching = true) {
            //sync data in Db & EF (if change not tracking for EF)
            //var ctx = ((IObjectContextAdapter) _context).ObjectContext;
            //ctx.Refresh(RefreshMode.StoreWins, ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified));
            //((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
            //_context.Entry(_dbSet.Where(predicate)).Reload();
            //_context.SaveChanges();
            Context.Configuration.AutoDetectChangesEnabled = enablecaching;
            var result = predicate == null ? _dbSet.FirstOrDefault() : _dbSet.FirstOrDefault(predicate);
            Context.Configuration.AutoDetectChangesEnabled = true;

            return result;
        }

        public void Add(T entity, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            _dbSet.Add(entity);
            Context.Configuration.AutoDetectChangesEnabled = true;
        }
        //http://entityframework-extensions.net/
        public void AddRange(IEnumerable<T> entityColl, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            _dbSet.AddRange(entityColl);
            Context.Configuration.AutoDetectChangesEnabled = true;
        }
        /// <summary>
        /// Mark all field of record as dirty => update all field (marking the whole entity as dirty)
        /// Work in disconnect scenario
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="detectChanges"></param>
        public void Update(T entity, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            //if context don't keep tracked entity
            //_dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            Context.Configuration.AutoDetectChangesEnabled = true;
        }

        /// <summary>
        /// Note: If context.Configuration.AutoDetectChangesEnabled = false then context cannot detect changes made to existing entities so do not execute update query.
        /// You need to call context.ChangeTracker.DetectChanges() before SaveChanges() in order to detect the edited entities and mark their status as Modified.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="detectChanges"></param>
        public void Update(T entity, Expression<Func<T, bool>> predicate, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            Context.Entry(Get(predicate)).State = EntityState.Modified;

            Context.Configuration.AutoDetectChangesEnabled = true;
        }
        public void Delete(Expression<Func<T, bool>> predicate, bool detectChanges = true) {
            var entitysRange = _dbSet.Where(predicate);

            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            foreach (var entity in entitysRange) {
                Context.Entry(entity).State = EntityState.Deleted;
            }
            Context.Configuration.AutoDetectChangesEnabled = true;
        }
        public void Merge(T entity, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            _dbSet.AddOrUpdate(entity);
            Context.Configuration.AutoDetectChangesEnabled = true;
        }
        public void Merge(IEnumerable<T> entityColl, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            foreach (var item in entityColl) {
                var entity = item;
                _dbSet.AddOrUpdate(entity);
            }

            Context.Configuration.AutoDetectChangesEnabled = true;
        }
        /// <summary>
        /// Merge only change values + exclude property (in develop)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="excludeFieds"></param>
        /// <param name="enablecaching"></param>
        public void Merge(T entity, Expression<Func<T, bool>> predicate, IEnumerable<string> excludeFieds, bool enablecaching = true) {
            Context.Configuration.AutoDetectChangesEnabled = enablecaching;

            if (_dbSet.Any(predicate.Compile())) {
                //connection scenario http://www.entityframeworktutorial.net/update-entity-in-entity-framework.aspx
                T item = _dbSet.Where(predicate).First();
                DbEntityEntry entry = Context.Entry(item);
                foreach (var propertyName in entry.OriginalValues.PropertyNames.Except(excludeFieds))
                {
                   //entry.CurrentValues.;
                }

               
                foreach (var propertyName in entry.OriginalValues.PropertyNames.Except(excludeFieds)) {
                    // Get the old field value from the database.
                    var original = entry.GetDatabaseValues().GetValue<object>(propertyName);
                    // Get the current value from posted edit page.
                    var current = entry.CurrentValues.GetValue<object>(propertyName);

                    if (!Equals(original, current)) {
                        entry.Property(propertyName).IsModified = true;
                    }
                }

            } else { Add(entity, false); }

            Context.Configuration.AutoDetectChangesEnabled = true;
        }

        public void Delete(T entity) {
            Context.Entry(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// if call saveChanges after change some property, this EntityState update only need field
        /// Work in disconnect scenario
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="detectChanges"></param>
        public void Edit(T entity, bool detectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = detectChanges;
            //_dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Unchanged;
            Context.Configuration.AutoDetectChangesEnabled = true;
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