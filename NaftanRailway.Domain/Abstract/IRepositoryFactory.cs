namespace NaftanRailway.Domain.Abstract
{
    public interface IRepositoryFactory<T>
        where T : class, new()
    {
        IRepository<T> Create();
    }
}