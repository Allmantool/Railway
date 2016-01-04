using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Abstract;

namespace NaftanRailway.Domain.Concrete {
    public sealed class UnitOfWork : IUnitOfWork {
        private bool _disposed;

        private System.Data.Entity.DbContext Contexts { get; set; }
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork(System.Data.Entity.DbContext context) {
            Contexts = context;
        }

        public UnitOfWork(params System.Data.Entity.DbContext[] contexts) {

        }

        /// <summary>
        /// Collection repositories
        /// </summary>
        public IGeneralRepository<T> Repository<T>() where T : class {
            if(_repositories.Keys.Contains(typeof(T)))
                return _repositories[typeof(T)] as IGeneralRepository<T>;

            IGeneralRepository<T> repo = new GeneralRepository<T>(Contexts);
            _repositories.Add(typeof(T), repo);

            return repo;
        }

        public void Save() {
            Contexts.SaveChanges();
        }
        private void Dispose(bool disposing) {
            if(!_disposed) {
                if(disposing)
                    Contexts.Dispose();
            }
            _disposed = true;
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
