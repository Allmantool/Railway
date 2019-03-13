using System.Data.Entity;
using Railway.Domain.Interfaces.Repositories;

namespace Railway.Domain.Interfaces
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
        DbContext ActiveDbContext { get; set; }

        bool Exists(object primaryKey);
    }
}