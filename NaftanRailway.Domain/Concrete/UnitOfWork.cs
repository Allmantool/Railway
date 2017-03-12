using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.Domain.Concrete {
    /*best approach that short live context (using)*/

    public class UnitOfWork : Disposable, IUnitOfWork {
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        /// <summary>
        ///     Create UOW per request with requirer dbContexts by default
        /// </summary>
        public UnitOfWork() {
            Contexts = new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() };
            SetUpContext();
        }

        /// <summary>
        ///     Constructor with specific dbContext
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(DbContext context) {
            ActiveContext = context;
            SetUpContext();
        }

        /// <summary>
        ///     Ninject (Dependency Injection). Pass a custom set of dbContext
        /// </summary>
        /// <param name="contexts"></param>
        public UnitOfWork(params DbContext[] contexts) {
            Contexts = contexts;
            SetUpContext();
        }

        private DbContext[] Contexts { get; }
        public DbContext ActiveContext { get; set; }

        /// <summary>
        ///     Collection repositories
        ///     Return repositories if it's in collection repositories, if not add in collection with specific dbcontext
        ///     Definition active dbcontext (depend on type of entity)
        /// </summary>
        public IGeneralRepository<T> Repository<T>() where T : class {
            if (_repositories.Keys.Contains(typeof(T)))
                return _repositories[typeof(T)] as IGeneralRepository<T>;

            //check exist entity in context(through metadata (reflection) in objectContext)
            if (Contexts != null)
                foreach (var contextItem in Contexts) {
                    var metaWorkspace = ((IObjectContextAdapter)contextItem).ObjectContext.MetadataWorkspace;
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
            //add new repositories
            IGeneralRepository<T> repo = new GeneralRepository<T>(ActiveContext);
            _repositories.Add(typeof(T), repo);

            return repo;
        }

        public void Save() {
            //TransactionScore score = new TransactionScore(); //old style
            using (var transaction = ActiveContext.Database.BeginTransaction()) {
                try {
                    //ActiveContext.ChangeTracker.DetectChanges();
                    ActiveContext.SaveChanges();
                    transaction.Commit();
                } catch (DbUpdateConcurrencyException ex) {
                    Console.WriteLine("Optimistic Concurrency exception occurred" + ex.Message);
                    transaction.Rollback();

                    throw new Exception("Error occurred in save method (Uow)");
                }
            }
        }

        public async Task SaveAsync() {
            using (var transaction = ActiveContext.Database.BeginTransaction()) {
                await ActiveContext.SaveChangesAsync();

                transaction.Commit();
            }
        }

        /// <summary>
        ///     Configuration setting of exist contexts
        /// </summary>
        /// <param name="lazyLoading"></param>
        /// <param name="proxy"></param>
        private void SetUpContext(bool lazyLoading = false, bool proxy = true) {
            /*Отключает Lazy loading необходим для Json (для сериализации entity to json*/
            foreach (var item in Contexts) {
                item.Configuration.LazyLoadingEnabled = lazyLoading;
                item.Configuration.ProxyCreationEnabled = proxy;
            }
        }

        protected override void DisposeCore() {
            if (ActiveContext != null)
                ActiveContext.Dispose();
        }
    }
}