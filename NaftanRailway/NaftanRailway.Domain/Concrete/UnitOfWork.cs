using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.Security;

namespace NaftanRailway.Domain.Concrete {
    public sealed class UnitOfWork : IUnitOfWork {
        private readonly System.Data.Entity.DbContext _context;

        private bool _disposed;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork(SimpleMemberShipDbEntities context) {
            _context = context;
        }

        public IGeneralRepository<T> Repository<T>() where T : class {
            if(_repositories.Keys.Contains(typeof(T)))
                return _repositories[typeof(T)] as IGeneralRepository<T>;

            IGeneralRepository<T> repo = new GeneralRepository<T>(_context);
            _repositories.Add(typeof(T), repo);

            return repo;
        }

        public void Save() {
            _context.SaveChanges();
        }

        private void Dispose(bool disposing) {
            if(!_disposed) {
                if(disposing)
                    _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
