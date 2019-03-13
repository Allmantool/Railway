namespace Railway.Domain.Interfaces
{
    public interface IRepositoryBuilder<T, TDbContext>
        where T : class, new()
        where TDbContext : class
    {
        IRepositoryBuilder<T, TDbContext> WithDbContext(TDbContext context);

        IRepository<T> Build();
    }
}
