using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using MoreLinq;
using NaftanRailway.Domain.Abstract;

namespace NaftanRailway.Domain.Concrete {
    public class GeneralRepository<T> : IGeneralRepository<T> where T : class {
        private bool _disposed;
        public System.Data.Entity.DbContext Context { get; }
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(System.Data.Entity.DbContext context) {
            Context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Get lazy data set (with cashing or not (attr MergeOption )
        /// </summary>
        /// <param name="predicate">filter condition for retriew data from source(database)</param>
        /// <param name="enableDetectChanges">Compare two snapshot of data (one when retriew data from database other when call method saveChanges(). If exists some diffrences => generate avaible SQL command</param>
        /// <param name="enableTracking"></param>
        /// <returns></returns>
        public IQueryable<T> Get_all(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true) {
            /*//sync data in Db & EF (if change not tracking for EF)
                ((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
                _context.Entry(_dbSet.Where(predicate)).Reload(); EF 4.1+*/
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            if (predicate == null) return (enableTracking) ? _dbSet : _dbSet.AsNoTracking();
            var result = (enableTracking) ? _dbSet.Where(predicate) : _dbSet.AsNoTracking().Where(predicate);

            return result;
        }
        public T Get(Expression<Func<T, bool>> predicate = null, bool enableDetectChanges = true, bool enableTracking = true) {
            //sync data in Db & EF (if change not tracking for EF)
            /*var ctx = ((IObjectContextAdapter) _context).ObjectContext;
            ctx.Refresh(RefreshMode.StoreWins, ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Modified));
            ((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
            _context.Entry(_dbSet.Where(predicate)).Reload();
            _context.SaveChanges();*/
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            if (predicate == null) return (enableTracking) ? _dbSet.FirstOrDefault() : _dbSet.AsNoTracking().FirstOrDefault();
            var result = (enableTracking) ? _dbSet.Where(predicate).FirstOrDefault() : _dbSet.AsNoTracking().Where(predicate).FirstOrDefault();

            return result;
        }
        /// <summary>
        /// Alternative method apposite Get. Diffrents => find first search in context.When not found in context then request to source(Db)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="enableDetectChanges"></param>
        /// <returns></returns>
        public T Find(dynamic key, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            return _dbSet.Find(key);
        }
        public void Add(T entity, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            _dbSet.Add(entity);
        }
        //http://entityframework-extensions.net/
        public void Add(IEnumerable<T> entityColl, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            _dbSet.AddRange(entityColl);
        }
        /// <summary>
        /// Mark all field of record as dirty => update all field (marking the whole entity as dirty)
        /// Work in disconnect scenario
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges"></param>
        public void Update(T entity, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            //if context don't keep tracked entity
            //_dbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }
        /// <summary>
        /// Note: If context.Configuration.AutoDetectChangesEnabled = false then context cannot detect changes made to existing entities so do not execute update query.
        /// You need to call context.ChangeTracker.DetectChanges() before SaveChanges() in order to detect the edited entities and mark their status as Modified.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="enableDetectChanges"></param>
        public void Update(T entity, Expression<Func<T, bool>> predicate, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            Context.Entry(Get(predicate)).State = EntityState.Modified;
            //_dbSet.Find(entity);
        }
        public void Merge(T entity, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            _dbSet.AddOrUpdate(entity);
        }
        public void Merge(IEnumerable<T> entityColl, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            entityColl.ForEach(x => _dbSet.AddOrUpdate(x));
        }
        /// <summary>
        /// Merge only change values + exclude property (in develop)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <param name="excludeFieds"></param>
        /// <param name="enableDetectChanges"></param>
        public void Merge(T entity, Expression<Func<T, bool>> predicate, IEnumerable<string> excludeFieds, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            //(Engage.Uow.ActiveContext.Entry(x).Property(p => p.DTBUHOTCHET).IsModified = true
            if (_dbSet.Any(predicate.Compile())) {
                //connection scenario http://www.entityframeworktutorial.net/update-entity-in-entity-framework.aspx
                T item = _dbSet.Where(predicate).First();
                DbEntityEntry entry = Context.Entry(item);
                foreach (var propertyName in entry.OriginalValues.PropertyNames.Except(excludeFieds)) {
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
        public void Delete(T entity, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            //if context don't keep tracked entity
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
            //Context.Entry(entity).State = EntityState.Deleted;
        }
        public void Delete(IEnumerable<T> entityColl, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            var list = entityColl.ToList();

            list.ForEach(x => _dbSet.Attach(x));
            _dbSet.RemoveRange(list);
        }
        /// <summary>
        /// Where we don't know key entity
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="enableDetectChanges"></param>
        public void Delete(Expression<Func<T, bool>> predicate, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            _dbSet.Where(predicate).ForEach(x => Context.Entry(x).State = EntityState.Deleted);
        }
        /// <summary>
        /// if call saveChanges after change some property, this EntityState update only need field
        /// Work in disconnect scenario
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enableDetectChanges"></param>
        public void Edit(T entity, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            Context.Entry(entity).State = EntityState.Unchanged;
        }
        public void Edit(IEnumerable<T> entityColl, Action<T> operations, bool enableDetectChanges = true) {
            Context.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            var list = entityColl.ToList();
            
            //list.ForEach(entity => _dbSet.Attach(entity));
            list.ForEach(entity => Context.Entry(entity).State = EntityState.Unchanged);
            list.ForEach(operations);
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