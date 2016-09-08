using System;
using System.Data.Entity;

namespace NaftanRailway.Domain.Abstract {
    public interface IUnitOfWork : IDisposable {
        /// <summary>
        /// Active DbContext (current)
        /// </summary>
        DbContext ActiveContext { get; set; }

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