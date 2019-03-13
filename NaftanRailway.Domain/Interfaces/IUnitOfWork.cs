using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Railway.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext ActiveContext { get; set; }

        DbContext[] Contexts { get; }

        IRepository<T> GetRepository<T>()
            where T : class, new();

        void Save();

        Task SaveAsync();
    }
}