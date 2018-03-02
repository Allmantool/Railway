namespace NaftanRailway.Domain.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Core;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NaftanRailway.Domain.Abstract;
    using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
    using NaftanRailway.Domain.Concrete.DbContexts.OBD;
    using NaftanRailway.Domain.Concrete.DbContexts.ORC;

    public class UnitOfWork : Disposable, IUnitOfWork
    {
        private DbContextTransaction transaction = null;

        private Dictionary<Type, IDisposable> mapRepositories;

        public UnitOfWork()
        {
            this.Contexts = new DbContext[]
            {
                new OBDEntities(),
                new MesplanEntities(),
                new ORCEntities()
            };

            this.SetUpContext();
        }

        public UnitOfWork(DbContext context)
        {
            this.ActiveContext = context;
            this.SetUpContext();
        }

        public UnitOfWork(params DbContext[] contexts)
        {
            this.Contexts = contexts;

            this.SetUpContext();
        }

        public DbContext[] Contexts { get; }

        public DbContext ActiveContext { get; set; }

        public IGeneralRepository<T> GetRepository<T>() where T : class
        {
            if (this.mapRepositories.TryGetValue(typeof(T), out var repo))
            {
                return repo as IGeneralRepository<T>;
            }

            if (Contexts != null)
            {
                foreach (var contextItem in Contexts)
                {
                    if (((IObjectContextAdapter)contextItem)
                        .ObjectContext
                        .MetadataWorkspace
                        .GetItems<EntityType>(DataSpace.CSpace)
                        .Any(w => w.Name == typeof(T).Name))
                    {
                        this.ActiveContext = contextItem;
                        this.ContextLog();

                        break;
                    }
                }
            }

            repo = new GeneralRepository<T>(ActiveContext);
            this.mapRepositories.Add(typeof(T), repo);

            return (IGeneralRepository<T>)repo;
        }

        public void Save()
        {
            try
            {
                ActiveContext.ChangeTracker.DetectChanges();
                ActiveContext.SaveChanges();

                this.transaction.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientEntry = entry.Entity;
                var databaseEntry = entry.GetDatabaseValues().ToObject();

                this.transaction.Rollback();

                throw new OptimisticConcurrencyException(
                    "Optimistic concurrency exception occurred during saving operation (Unit of work)." +
                    $"Transaction was rolled backed. Message: {ex.Message}." +
                    $"Database type: {databaseEntry}." +
                    $"Client type: {clientEntry}.");
            }
            finally
            {
                this.transaction = ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot);
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await ActiveContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.transaction.Rollback();

                throw new OptimisticConcurrencyException(
                    "Optimistic concurrency exception occurred during saving async operation (Unit of work)." +
                    $"Message: {ex.Message}");
            }


            this.transaction.Commit();
        }

        /// <summary>
        /// Configuration setting of exist contexts.
        /// </summary>
        /// <param name="lazyLoading"></param>
        /// <param name="proxy"></param>
        private void SetUpContext(bool lazyLoading = false, bool proxy = true)
        {
            /* Disable Lazy loading (for entity to json) */
            foreach (var item in Contexts)
            {
                item.Configuration.LazyLoadingEnabled = lazyLoading;
                item.Configuration.ProxyCreationEnabled = proxy;
            }

            this.mapRepositories = new Dictionary<Type, IDisposable>();

            this.transaction = ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot);
        }

        private void ContextLog()
        {
            if (ActiveContext != null)
            {
                ActiveContext.Database.Log = (s => Debug.WriteLine(s));
                ActiveContext.Database.Log = message => Trace.Write(message);
                ActiveContext.Database.Log = (Console.WriteLine);
            }
        }

        protected override void ExtenstionDispose()
        {
            ActiveContext?.Dispose();
            Dispose();
        }
    }
}