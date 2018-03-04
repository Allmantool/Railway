namespace NaftanRailway.Domain.Abstract
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        DbContext ActiveContext { get; set; }

        DbContext[] Contexts { get; }

        IRepository<T> GetRepository<T>() where T : class;

        void Save();

        Task SaveAsync();
    }
}