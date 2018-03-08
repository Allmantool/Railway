namespace NaftanRailway.Domain.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Abstract;

    public class EntityFrameworkRepositoryFactory<T> : IRepositoryFactory<IRepository<T>, T>
        where T : class
    {
        private readonly DbContext[] dbCtxCollection;

        private readonly Dictionary<Type, IDisposable> mapRepositories;

        public EntityFrameworkRepositoryFactory(DbContext[] dbCtxCollection)
        {
            this.dbCtxCollection = dbCtxCollection;
        }

        public IRepository<T> Create()
        {
            if (this.mapRepositories.TryGetValue(typeof(T), out var repo))
            {
                return repo as IRepository<T>;
            }

            if (this.dbCtxCollection != null)
            {
                foreach (var contextItem in this.dbCtxCollection)
                {
                    if (((IObjectContextAdapter)contextItem)
                        .ObjectContext
                        .MetadataWorkspace
                        .GetItems<EntityType>(DataSpace.CSpace)
                        .Any(w => w.Name == typeof(T).Name))
                    {
                        repo = new Repository<T>(contextItem);
                        this.mapRepositories.Add(typeof(T), repo);

                        break;
                    }
                }
            }

            return (IRepository<T>)repo;
        }
    }
}
