using System;
using System.Threading.Tasks;
using Railway.Core.Data.Interfaces.Repositories;

namespace Railway.Core.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>()
            where T : class;

        int Save();

        Task<int> SaveAsync();
    }
}