using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Abstract;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.Concrete {
    /*best approach that short live context (using)*/
    public class UnitOfWork : IUnitOfWork {
        private bool _disposed;
        public System.Data.Entity.DbContext ActiveContext { get; set; }
        private System.Data.Entity.DbContext[] Contexts { get; set; }
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        /// <summary>
        /// Create UOW per request with requer dbContexts by default
        /// </summary>
        public UnitOfWork() {
            Contexts = new System.Data.Entity.DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() };
        }
        /// <summary>
        /// Constructor with specific dbContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="lazyLoading"></param>
        /// <param name="proxy"></param>
        public UnitOfWork(System.Data.Entity.DbContext context, bool lazyLoading = true, bool proxy = true) {
            ActiveContext = context;
            /*Отключает Lazy loading необходим для Json (для сериализации entity to json*/
            ActiveContext.Configuration.LazyLoadingEnabled = lazyLoading;
            ActiveContext.Configuration.ProxyCreationEnabled = proxy;
        }
        /// <summary>
        /// Ninject (Dependency Injection). Pass a custom set of dbContext
        /// </summary>
        /// <param name="contexts"></param>
        public UnitOfWork(params System.Data.Entity.DbContext[] contexts) {
            Contexts = contexts;
        }

        /// <summary>
        /// Collection repositories
        /// Return repositories if it's in collection repositories, if not add in collection with specific dbcontext
        /// Definition active dbcontext (depend on type of entity)
        /// </summary>
        public IGeneralRepository<T> Repository<T>() where T : class {
            if (_repositories.Keys.Contains(typeof(T)))
                return _repositories[typeof(T)] as IGeneralRepository<T>;

            //check exist entity in context(through metadata (reflection) in objectContext)
            if (Contexts != null) {
                foreach (var contextItem in Contexts) {
                    MetadataWorkspace metaWorkspace = ((IObjectContextAdapter)contextItem).ObjectContext.MetadataWorkspace;
                    //reflection (search by name in object metadata)
                    if (metaWorkspace.GetItems<EntityType>(DataSpace.CSpace).Any(w => w.Name == typeof(T).Name)) {
                        ActiveContext = contextItem;
                        /*log for EF6 dbcontext in output window (debug mode)*/
                        //ActiveContext.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));
                        //ActiveContext.Database.Log = message => Trace.Write(message);
                        //ActiveContext.Database.Log = (Console.WriteLine);
                        break;
                    }
                }
            }
            //add new repositories
            IGeneralRepository<T> repo = new GeneralRepository<T>(ActiveContext);
            _repositories.Add(typeof(T), repo);

            return repo;
        }
        public void Save() {
            using (var transaction = ActiveContext.Database.BeginTransaction()) {
                try {
                    //ActiveContext.ChangeTracker.DetectChanges();
                    ActiveContext.SaveChanges();
                    transaction.Commit();
                } catch (DbUpdateConcurrencyException ex) {
                    Console.WriteLine("Optimistic Concurrency exception occured" + ex.Message);
                    transaction.Rollback();

                    throw new Exception("Error occured in save method (Uow)");
                }
            }
        }

        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing && (ActiveContext != null)) {
                    ActiveContext.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}