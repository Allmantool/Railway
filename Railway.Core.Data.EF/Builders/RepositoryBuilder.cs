namespace Railway.Core.Data.EF.Builders
{
    using System.Data.Entity;
    using Railway.Core.Data.Interfaces.Builders;
    using Interfaces.Repositories;

    public class EFRepositoryBuilder<T, TDbContext> : IRepositoryBuilder<T, TDbContext>
        where T : class, new()
        where TDbContext : DbContext, new()
    {
        public IRepository<T> Build()
        {
            throw new System.NotImplementedException();
        }

        public IRepositoryBuilder<T, TDbContext> WithDbContext(TDbContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
