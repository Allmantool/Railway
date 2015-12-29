using System;

namespace NaftanRailway.Domain.Abstract {
    public interface IUnitOfWork : IDisposable {
        IGeneralRepository<T> Repository<T>() where T : class;
        void Save();
    }
}
