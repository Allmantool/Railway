namespace NaftanRailway.Domain.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstract;
    using MoreLinq;

    public class Repository<T> : Disposable, IRepository<T>
        where T : class
    {
        private readonly DbSet<T> dbSet;

        public Repository(DbContext context)
        {
            this.ActiveDbContext = context;
            this.dbSet = context.Set<T>();
        }

        public DbContext ActiveDbContext { get; set; }

        public IQueryable<T> GetAll(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true)
        {
            /* Sync data in Db & EF (if change not tracking for EF)
                ((IObjectContextAdapter)_context).ObjectContext.Refresh(RefreshMode.StoreWins, _dbSet.Where(predicate));
                _context.Entry(_dbSet.Where(predicate)).Reload(); EF 4.1+
            */

            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            predicate = predicate ?? (x => true);

            var result = enableTracking
                ? this.dbSet.Where(predicate)
                : this.dbSet.AsNoTracking().Where(predicate);

            return result;
        }

        public T Get(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            predicate = predicate ?? (x => true);

            var result = enableTracking
                ? this.dbSet.Where(predicate).SingleOrDefault()
                : this.dbSet.AsNoTracking().Where(predicate).SingleOrDefault();

            return result;
        }

        public async Task<T> GetAsync(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            predicate = predicate ?? (x => true);

            var result = enableTracking
                ? await this.dbSet.Where(predicate).SingleOrDefaultAsync()
                : await this.dbSet.AsNoTracking().Where(predicate).SingleOrDefaultAsync();

            return result;
        }

        public T Find<TK>(TK key, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            return this.dbSet.Find(key);
        }

        public async Task<T> FindAsync<TK>(TK key, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            return await this.dbSet.FindAsync(key);
        }

        public void Add(T entity, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            this.dbSet.Add(entity);
        }

        public void Add(IQueryable<T> entityColl, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            this.dbSet.AddRange(entityColl);
        }

        public void Update(
            Expression<Func<T, bool>> predicate,
            IEnumerable<Action<T>> operations,
            bool enableDetectChanges = true)
        {
            this.Update(
                this.GetAll(predicate),
                operations,
                enableDetectChanges);
        }

        public void Update(
            IQueryable<T> entities,
            IEnumerable<Action<T>> operations,
            bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            foreach (var item in entities)
            {
                if (!this.IsTracked(item))
                {
                    this.dbSet.Attach(item);
                }
            }

            entities.ForEach(x => this.Update(x, operations, enableDetectChanges));
        }

        public void Update(
            T entity,
            IEnumerable<Action<T>> operations,
            bool enableDetectChanges = true)
        {
            operations.ForEach(o => o(entity));

            // Do a database call to get the state
            var entry = this.ActiveDbContext.Entry(entity);
            var databaseValues = entry.GetDatabaseValues();

            foreach (var propertyName in databaseValues.PropertyNames)
            {
                // Modify the specific property states only.
                entry.Property(propertyName).IsModified = true;
            }
        }

        public void Merge(T entity, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            this.dbSet.AddOrUpdate(entity);
        }

        public void Merge(IQueryable<T> entityColl, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            entityColl.ForEach(x => this.dbSet.AddOrUpdate(x));
        }

        public void Merge(
            T entity,
            Expression<Func<T, bool>> predicate,
            IQueryable<string> excludeFieds,
            bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            if (this.dbSet.Any(predicate.Compile()))
            {
                // connection scenario http://www.entityframeworktutorial.net/update-entity-in-entity-framework.aspx
                var item = this.dbSet.Where(predicate).First();
                DbEntityEntry entry = this.ActiveDbContext.Entry(item);

                foreach (var propertyName in entry.OriginalValues.PropertyNames.Except(excludeFieds))
                {
                    // Get the old field value from the database.
                    var original = entry.GetDatabaseValues().GetValue<object>(propertyName);

                    // Get the current value from posted edit page.
                    var current = entry.CurrentValues.GetValue<object>(propertyName);

                    if (!Equals(original, current))
                    {
                        entry.Property(propertyName).IsModified = true;
                    }
                }
            }
            else
            {
                this.Add(entity, false);
            }

            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = true;
        }

        public bool Exists(object primaryKey)
        {
            return this.dbSet.Find(primaryKey) != null;
        }

        public void Delete<TK>(TK key, bool enableDetectChanges = true)
        {
            this.dbSet.Remove(this.Find(key, enableDetectChanges));
        }

        public void Delete(T entity, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            if (!this.IsTracked(entity))
            {
                this.dbSet.Attach(entity);
            }

            this.ActiveDbContext.Entry(entity).State = EntityState.Deleted;
        }

        public void Delete(IQueryable<T> entityColl, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;

            foreach (var item in entityColl)
            {
                if (!this.IsTracked(item))
                {
                    this.dbSet.Attach(item);
                }
            }

            this.dbSet.RemoveRange(entityColl);
        }

        public void Delete(Expression<Func<T, bool>> predicate, bool enableDetectChanges = true)
        {
            this.ActiveDbContext.Configuration.AutoDetectChangesEnabled = enableDetectChanges;
            this.dbSet.Where(predicate).ForEach(x => this.ActiveDbContext.Entry(x).State = EntityState.Deleted);
        }

        protected override void ExtenstionDispose()
        {
            this.ActiveDbContext?.Dispose();
        }

        private bool IsTracked(T entity)
        {
            return this.dbSet.Local.Any(e => e == entity);
        }
    }
}