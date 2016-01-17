﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using NaftanRailway.Domain.Abstract;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace NaftanRailway.Domain.Concrete {
    public sealed class UnitOfWork : IUnitOfWork {
        private bool _disposed;

        public System.Data.Entity.DbContext ActiveContext { get; private set; }
        private System.Data.Entity.DbContext[] Contexts { get; set; }
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork() {
        }
        public UnitOfWork(System.Data.Entity.DbContext context) {
            ActiveContext = context;
        }
        public UnitOfWork(params System.Data.Entity.DbContext[] contexts) {
            Contexts = contexts;
        }

        /// <summary>
        /// Collection repositories
        /// Return repositories if it's in collection repositories, if not add in collection with specific db context
        /// </summary>
        public IGeneralRepository<T> Repository<T>() where T : class {
            if(_repositories.Keys.Contains(typeof(T)))
                return _repositories[typeof(T)] as IGeneralRepository<T>;

            //check exist entity in context(through metadata in objectContext)
            if(Contexts != null) {
                foreach(var contextItem in Contexts) {
                    ObjectContext objContext = ((IObjectContextAdapter)contextItem).ObjectContext;
                    MetadataWorkspace workspace = objContext.MetadataWorkspace;

                    if(workspace.GetItems<EntityType>(DataSpace.CSpace).Any(w => w.Name == typeof(T).Name)) {
                        ActiveContext = contextItem;
                        break;
                    }
                }
            }

            IGeneralRepository<T> repo = new GeneralRepository<T>(ActiveContext);
            _repositories.Add(typeof(T), repo);

            return repo;
        }

        public void Save() {
            ActiveContext.SaveChanges();
        }
        private void Dispose(bool disposing) {
            if(!_disposed) {
                if(disposing)
                    ActiveContext.Dispose();
            }
            _disposed = true;
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}