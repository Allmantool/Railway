namespace Railway.Core.Data.EF.Factories
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Railway.Core.Data.Interfaces.Builders;
    using Railway.Core.Data.Interfaces.Factories;
    using Interfaces.Repositories;

    public class RepositoryFactory<T> : IRepositoryFactory<T>
        where T : class, new()
    {
        private readonly string typeName;
        private readonly IEnumerable<DbContext> dbCtxCollection;
        private readonly IDictionary<string, IRepository<T>> mapRepositories;
        private readonly IRepositoryBuilder<T, DbContext> repositoryBuilder;

        public RepositoryFactory(IRepositoryBuilder<T, DbContext> repositoryBuilder, IEnumerable<DbContext> dbCtxCollection)
        {
            typeName = typeof(T).Name;
            mapRepositories = new Dictionary<string, IRepository<T>>();
            this.repositoryBuilder = repositoryBuilder;
            this.dbCtxCollection = dbCtxCollection;
        }

        public IRepository<T> Create()
        {
            if (mapRepositories.TryGetValue(typeName, out var repo))
            {
                return repo;
            }

            if (dbCtxCollection != null)
            {
                foreach (var contextItem in dbCtxCollection)
                {
                    if (IsEntityBelongsToDbContext(contextItem))
                    {
                        repo = repositoryBuilder
                            .WithDbContext(contextItem)
                            .Build();

                        mapRepositories.Add(typeName, repo);

                        break;
                    }
                }
            }

            return repo;
        }

        private bool IsEntityBelongsToDbContext(DbContext dbContext)
        {
            return ((IObjectContextAdapter)dbContext)
                        .ObjectContext
                        .MetadataWorkspace
                        .GetItems<EntityType>(DataSpace.CSpace)
                        .Any(w => w.Name == typeName);
        }
    }
}
