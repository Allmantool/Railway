namespace Railway.Domain.Interfaces
{
    public interface IUnitOfWorkFactory<out TUnitOfWork>
        where TUnitOfWork : IUnitOfWork
    {
        TUnitOfWork Create();
    }
}