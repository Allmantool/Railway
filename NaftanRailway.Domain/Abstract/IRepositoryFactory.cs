namespace NaftanRailway.Domain.Abstract
{
    using System.Data.Entity;

    public interface IRepositoryFactory<out TRepository, T>
        where TRepository : IRepository<T>
    {
        TRepository Create();
    }
}