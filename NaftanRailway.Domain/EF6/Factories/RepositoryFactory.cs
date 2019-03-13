namespace Railway.Domain.EF6.Factories
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Interfaces;

    public class RepositoryFactory<T> : IRepositoryFactory<T>
        where T : class, new()
    {
        private readonly string typeName;
        private readonly IEnumerable<DbContext> dbCtxCollection;
        private readonly IDictionary<string, IRepository<T>> mapRepositories;
        private readonly IRepositoryBuilder<T, DbContext> repositoryBuilder;

        public RepositoryFactory(IRepositoryBuilder<T, DbContext> repositoryBuilder, IEnumerable<DbContext> dbCtxCollection)
        {
            this.typeName = typeof(T).Name;
            this.mapRepositories = new Dictionary<string, IRepository<T>>();
            this.repositoryBuilder = repositoryBuilder;
            this.dbCtxCollection = dbCtxCollection;
        }

        public IRepository<T> Create()
        {
            if (this.mapRepositories.TryGetValue(this.typeName, out var repo))
            {
                return repo;
            }

            if (this.dbCtxCollection != null)
            {
                foreach (var contextItem in this.dbCtxCollection)
                {
                    if (this.IsEntityBelongsToDbContext(contextItem))
                    {
                        repo = this.repositoryBuilder
                            .WithDbContext(contextItem)
                            .Build();

                        this.mapRepositories.Add(this.typeName, repo);

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
                        .Any(w => w.Name == this.typeName);
        }
    }
}
