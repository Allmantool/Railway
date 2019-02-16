namespace NaftanRailway.Domain.Abstract
{
    public interface IUnitOfWorkFactory<out TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        TUnitOfWork Create();
    }
}