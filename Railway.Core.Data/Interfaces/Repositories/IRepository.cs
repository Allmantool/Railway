namespace Railway.Core.Data.Interfaces.Repositories
{
    public interface IRepository<T> :
        IAsyncRepository<T>,
        IGetRepository<T>,
        IAddRepository<T>,
        IDeleteRepository<T>,
        IUpdateRepository<T>,
        IMergeRepository<T>
     where T : class, new()
    {
        bool Exists(object primaryKey);
    }
}