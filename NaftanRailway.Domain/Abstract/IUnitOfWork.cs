using System;

namespace NaftanRailway.Domain.Abstract {
    public interface IUnitOfWork : IDisposable {
        System.Data.Entity.DbContext ActiveContext { get; }
        IGeneralRepository<T> Repository<T>() where T : class;
        void Save();
    }
}
