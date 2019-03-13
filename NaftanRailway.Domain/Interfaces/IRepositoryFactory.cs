namespace Railway.Domain.Interfaces
{
    public interface IRepositoryFactory<T>
        where T : class, new()
    {
        IRepository<T> Create();
    }
}