namespace Railway.Core.Data.EF
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
    using Interfaces;
    using Interfaces.Repositories;

    public class EFUnitOfWork : Disposable, IUnitOfWork
    {
        private DbContextTransaction transaction;

        private Dictionary<Type, IDisposable> mapRepositories;

        public EFUnitOfWork(DbContext context)
        {
            this.ActiveContext = context;
            this.SetUpContext();
        }

        public EFUnitOfWork(params DbContext[] contexts)
        {
            this.Contexts = contexts;
            this.SetUpContext();
        }

        public DbContext[] Contexts { get; }

        public DbContext ActiveContext { get; set; }

        public IRepository<T> GetRepository<T>()
            where T : class, new()
        {
            if (this.mapRepositories.TryGetValue(typeof(T), out var repo))
            {
                return repo as IRepository<T>;
            }

            if (this.Contexts != null)
            {
                foreach (var contextItem in this.Contexts)
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

            repo = new Repository<T>(this.ActiveContext);
            this.mapRepositories.Add(typeof(T), repo);

            return (IRepository<T>)repo;
        }

        public int Save()
        {
            int affectedRowsCount;
            try
            {
                this.ActiveContext.ChangeTracker.DetectChanges();
                affectedRowsCount = this.ActiveContext.SaveChanges();

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
                this.transaction = this.ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot);
            }

            return affectedRowsCount;
        }

        public async Task<int> SaveAsync()
        {
            Task<int> affectedRowsCountAsync;
            try
            {
                affectedRowsCountAsync = this.ActiveContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                this.transaction.Rollback();

                throw new OptimisticConcurrencyException(
                    "Optimistic concurrency exception occurred during saving async operation (Unit of work)." +
                    $"Message: {ex.Message}");
            }

            this.transaction.Commit();

            return await affectedRowsCountAsync;
        }

        protected override void ExtenstionDispose()
        {
            this.ActiveContext?.Dispose();
            this.Dispose();
        }

        private void SetUpContext(bool lazyLoading = false, bool proxy = true)
        {
            /* Disable Lazy loading (for entity to json) */
            foreach (var item in this.Contexts)
            {
                item.Configuration.LazyLoadingEnabled = lazyLoading;
                item.Configuration.ProxyCreationEnabled = proxy;
            }

            this.mapRepositories = new Dictionary<Type, IDisposable>();

            this.transaction = this.ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot);
        }

        private void ContextLog()
        {
            if (this.ActiveContext != null)
            {
                this.ActiveContext.Database.Log = s => Debug.WriteLine(s);
                this.ActiveContext.Database.Log = message => Trace.Write(message);
                this.ActiveContext.Database.Log = Console.WriteLine;
            }
        }
    }
}