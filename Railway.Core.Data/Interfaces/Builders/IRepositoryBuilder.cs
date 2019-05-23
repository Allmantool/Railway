namespace Railway.Core.Data.Interfaces.Builders
{
    using Repositories;

    public interface IRepositoryBuilder<T, in TDbContext>
        where T : class, new()
        where TDbContext : class
    {
        IRepositoryBuilder<T, TDbContext> WithDbContext(TDbContext context);

        IRepository<T> Build();
    }
}
