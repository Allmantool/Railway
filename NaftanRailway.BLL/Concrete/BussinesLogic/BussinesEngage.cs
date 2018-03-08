namespace NaftanRailway.BLL.Concrete.BussinesLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using log4net;
    using NaftanRailway.BLL.Abstract;
    using NaftanRailway.Domain.Abstract;
    using NaftanRailway.Domain.Concrete;

    public sealed class BussinesEngage : Disposable, IBussinesEngage
    {
        public BussinesEngage(IUnitOfWork unitOfWork, ILog log)
        {
            this.Uow = unitOfWork;
            this.Log = log;
        }

        public ILog Log { get; }

        public IUnitOfWork Uow { get; set; }

        /// <summary>
        /// Get General table with predicate.
        /// </summary>
        public IEnumerable<T> GetTable<T, TKey>(
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, TKey>> orderPredicate = null,
            bool caсhe = false,
            bool tracking = false)
            where T : class
        {
            using (Uow = new UnitOfWork())
            {
                return (orderPredicate == null) 
                    ? Uow.GetRepository<T>().GetAll(predicate, caсhe, tracking).ToList()
                    : Uow.GetRepository<T>().GetAll(predicate, caсhe, tracking).OrderByDescending(orderPredicate).ToList();
            }
        }

        public long GetCountRows<T>(Expression<Func<T, bool>> predicate = null)
            where T : class
        {
            using (Uow = new UnitOfWork())
            {
                return Uow.GetRepository<T>()
                    .GetAll(predicate, false, false)
                    .Count();
            }
        }

        public IEnumerable<T> GetSkipRows<T, TKey>(
            int page,
            int size,
            Expression<Func<T, TKey>> orderPredicate,
            Expression<Func<T, bool>> filterPredicate = null,
            bool caсhe = false)
            where T : class
        {
            using (Uow = new UnitOfWork())
            {
                return Uow.GetRepository<T>()
                    .GetAll(filterPredicate, caсhe)
                    .OrderByDescending(orderPredicate).Skip((page - 1) * size)
                    .Take(size).ToList();
            }
        }

        /// <summary>
        /// Custom group function
        /// </summary>
        /// <typeparam name="T">A element type (entity, table, object)</typeparam>
        /// <typeparam name="TKey">It's key by which grouping sequence of type T</typeparam>
        /// <param name="groupPredicate">It's predicate for grouping</param>
        /// <param name="predicate">It's predicate for filtering</param>
        /// <param name="caсhe"></param>
        /// <returns>It returns IEnumerable'IGrouping' </returns>
        public IEnumerable<IGrouping<TKey, T>> GetGroup<T, TKey>(
            Expression<Func<T, TKey>> groupPredicate,
            Expression<Func<T, bool>> filterPredicate = null,
            Expression<Func<T, TKey>> orderPredicate = null,
            bool caсhe = false)
            where T : class
        {
            using (Uow = new UnitOfWork())
            {
                IList<IGrouping<TKey, T>> result;
                try
                {
                    result = Uow.GetRepository<T>().GetAll(filterPredicate, caсhe)
                        .OrderBy(orderPredicate ?? groupPredicate)
                        .GroupBy(groupPredicate)
                        .ToList();
                }
                catch (Exception ex)
                {
                    Log.DebugFormat($"GetGroup LINQ custom method throws exception: {ex.Message}.");
                    throw;
                }

                return result;
            }
        }

        protected override void ExtenstionDispose() => this.Uow?.Dispose();
    }
}