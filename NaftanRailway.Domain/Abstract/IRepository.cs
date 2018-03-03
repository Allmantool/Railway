namespace NaftanRailway.Domain.Abstract
{
    using System.Data.Entity;

    using NaftanRailway.Domain.Abstract.Repositories;

    /// <summary>
    /// The Repository interface.
    /// </summary>
    /// <typeparam name="T"> Generic entity.</typeparam>
    public interface IRepository<T> :
        IAsyncRepository<T>,
        IGetRepository<T>,
        IAddRepository<T>,
        IDeleteRepository<T>,
        IUpdateRepository<T>,
        IMergeRepository<T>
    {
        DbContext ActiveDbContext { get; set; }

        bool Exists(object primaryKey);
    }
}