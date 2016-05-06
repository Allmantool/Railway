using System;

namespace NaftanRailway.Domain.Abstract {
    public interface IUnitOfWork : IDisposable {
        /// <summary>
        /// Active DbContext (current)
        /// </summary>
        System.Data.Entity.DbContext ActiveContext { get; }
        /// <summary>
        /// General type repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IGeneralRepository<T> Repository<T>() where T : class;
        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
