using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.Domain.Concrete
{
    public class UnitOfWork : Disposable, IUnitOfWork
    {
        public DbContext[] Contexts { get; }
        public DbContext ActiveContext { get; set; }

        private Dictionary<Type, IDisposable> _mapRepositories;

        public UnitOfWork()
        {
            Contexts = new DbContext[]
            {
                new OBDEntities(),
                new MesplanEntities(),
                new ORCEntities()
            };
            SetUpContext();
        }

        public UnitOfWork(DbContext context)
        {
            ActiveContext = context;
            SetUpContext();
        }

        public UnitOfWork(params DbContext[] contexts)
        {
            Contexts = contexts;// new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() };
            SetUpContext();
        }

        public IGeneralRepository<T> GetRepository<T>() where T : class
        {
            if (_mapRepositories.TryGetValue(typeof(T), out var repo))
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
                        ActiveContext = contextItem;
                        ContextLog();

                        break;
                    }
                }
            }

            repo = new GeneralRepository<T>(ActiveContext);
            _mapRepositories.Add(typeof(T), repo);

            return (IGeneralRepository<T>)repo;
        }

        public void Save()
        {
            using (var transaction = ActiveContext.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    ActiveContext.ChangeTracker.DetectChanges();
                    ActiveContext.SaveChanges();

                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientEntry = entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues().ToObject();

                    transaction.Rollback();

                    throw new OptimisticConcurrencyException("Optimistic concurrency exception occurred during saving operation (Unit of work)." +
                                                             $"Transaction was rolled backed. Message: {ex.Message}." +
                                                             $"Database type: {databaseEntry}." +
                                                             $"Client type: {clientEntry}.");
                }
            }
        }

        public async Task SaveAsync()
        {
            using (var transaction = ActiveContext.Database.BeginTransaction())
            {
                await ActiveContext.SaveChangesAsync();
                transaction.Commit();
            }
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

            _mapRepositories = new Dictionary<Type, IDisposable>();
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

        protected override void DisposeCore()
        {
            ActiveContext?.Dispose();
        }
    }
}