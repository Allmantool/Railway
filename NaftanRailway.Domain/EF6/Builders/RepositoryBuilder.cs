namespace Railway.Domain.EF6.Builders
{
    using System.Data.Entity;
    using Interfaces;

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
