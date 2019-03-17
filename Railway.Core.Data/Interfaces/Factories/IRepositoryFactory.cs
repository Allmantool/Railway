namespace Railway.Core.Data.Interfaces.Factories
{
    using Repositories;

    public interface IRepositoryFactory<T>
        where T : class, new()
    {
        IRepository<T> Create();
    }
}