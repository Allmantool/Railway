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

    public class UnitOfWork : Disposable, IUnitOfWork
    {
        private DbContextTransaction _transaction;

        private Dictionary<Type, IDisposable> _mapRepositories;

        public UnitOfWork(DbContext context)
        {
            ActiveContext = context;
            SetUpContext();
        }

        public UnitOfWork(params DbContext[] contexts)
        {
            Contexts = contexts;
            SetUpContext();
        }

        public DbContext[] Contexts { get; }

        public DbContext ActiveContext { get; set; }

        public IRepository<T> GetRepository<T>()
            where T : class
        {
            if (_mapRepositories.TryGetValue(typeof(T), out var repo))
            {
                return repo as IRepository<T>;
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
                        ActiveContext = contextItem;
                        ContextLog();

                        break;
                    }
                }
            }

            repo = new Repository<T>(ActiveContext);
            _mapRepositories.Add(typeof(T), repo);

            return (IRepository<T>)repo;
        }

        public int Save()
        {
            int affectedRowsCount;
            try
            {
                ActiveContext.ChangeTracker.DetectChanges();
                affectedRowsCount = ActiveContext.SaveChanges();

                _transaction.Commit();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var clientEntry = entry.Entity;
                var databaseEntry = entry.GetDatabaseValues().ToObject();

                _transaction.Rollback();

                throw new OptimisticConcurrencyException(
                    "Optimistic concurrency exception occurred during saving operation (Unit of work)." +
                    $"Transaction was rolled backed. Message: {ex.Message}." +
                    $"Database type: {databaseEntry}." +
                    $"Client type: {clientEntry}.");
            }
            finally
            {
                _transaction = ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot);
            }

            return affectedRowsCount;
        }

        public async Task<int> SaveAsync()
        {
            Task<int> affectedRowsCountAsync;
            try
            {
                affectedRowsCountAsync = ActiveContext.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _transaction.Rollback();

                throw new OptimisticConcurrencyException(
                    "Optimistic concurrency exception occurred during saving async operation (Unit of work)." +
                    $"Message: {ex.Message}");
            }

            _transaction.Commit();

            return await affectedRowsCountAsync;
        }

        protected override void ExtenstionDispose()
        {
            ActiveContext?.Dispose();
            Dispose();
        }

        private void SetUpContext(bool lazyLoading = false, bool proxy = true)
        {
            /* Disable Lazy loading (for entity to json) */
            foreach (var item in Contexts)
            {
                item.Configuration.LazyLoadingEnabled = lazyLoading;
                item.Configuration.ProxyCreationEnabled = proxy;
            }

            _mapRepositories = new Dictionary<Type, IDisposable>();

            _transaction = ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot);
        }

        private void ContextLog()
        {
            if (ActiveContext != null)
            {
                ActiveContext.Database.Log = s => Debug.WriteLine(s);
                ActiveContext.Database.Log = message => Trace.Write(message);
                ActiveContext.Database.Log = Console.WriteLine;
            }
        }
    }
}