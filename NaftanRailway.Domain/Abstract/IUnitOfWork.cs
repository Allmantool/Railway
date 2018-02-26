using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace NaftanRailway.Domain.Abstract {
    public interface IUnitOfWork : IDisposable {
        /// <summary>
        ///     Active DbContext (current)
        /// </summary>
        DbContext ActiveContext { get; set; }
        DbContext[] Contexts { get; }

        /// <summary>
        ///     General type repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IGeneralRepository<T> GetRepository<T>() where T : class;

        /// <summary>
        ///     Save method
        /// </summary>
        void Save();

        Task SaveAsync();
    }
}