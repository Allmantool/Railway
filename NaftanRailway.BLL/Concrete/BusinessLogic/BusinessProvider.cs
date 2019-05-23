namespace NaftanRailway.BLL.Concrete.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using log4net;
    using Abstract;
    using Railway.Core;
    using Railway.Core.Data.EF;

    public sealed class BusinessProvider : Disposable, IBusinessProvider
    {
        public BusinessProvider(UnitOfWork unitOfWork, ILog log)
        {
            this.Uow = unitOfWork;
            this.Log = log;
        }

        public ILog Log { get; }

        public UnitOfWork Uow { get; set; }

        public IEnumerable<T> GetTable<T, TKey>(
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, TKey>> orderPredicate = null,
            bool cache = false,
            bool tracking = false)
            where T : class
        {
            using (this.Uow = new UnitOfWork())
            {
                return (orderPredicate == null) 
                    ? this.Uow.GetRepository<T>().GetAll(predicate, cache, tracking).ToList()
                    : this.Uow.GetRepository<T>().GetAll(predicate, cache, tracking).OrderByDescending(orderPredicate).ToList();
            }
        }

        public long GetCountRows<T>(Expression<Func<T, bool>> predicate = null)
            where T : class
        {
            using (this.Uow = new UnitOfWork())
            {
                return this.Uow.GetRepository<T>()
                    .GetAll(predicate, false, false)
                    .Count();
            }
        }

        public IEnumerable<T> GetSkipRows<T, TKey>(
            int page,
            int size,
            Expression<Func<T, TKey>> orderPredicate,
            Expression<Func<T, bool>> filterPredicate = null,
            bool cache = false)
            where T : class
        {
            using (this.Uow = new UnitOfWork())
            {
                return this.Uow.GetRepository<T>()
                    .GetAll(filterPredicate, cache)
                    .OrderByDescending(orderPredicate).Skip((page - 1) * size)
                    .Take(size).ToList();
            }
        }

        public IEnumerable<IGrouping<TKey, T>> GetGroup<T, TKey>(
            Expression<Func<T, TKey>> groupPredicate,
            Expression<Func<T, bool>> filterPredicate = null,
            Expression<Func<T, TKey>> orderPredicate = null,
            bool cache = false)
            where T : class
        {
            using (this.Uow = new UnitOfWork())
            {
                IList<IGrouping<TKey, T>> result;
                try
                {
                    result = this.Uow.GetRepository<T>()
                        .GetAll(filterPredicate, cache)
                        .OrderBy(orderPredicate ?? groupPredicate)
                        .GroupBy(groupPredicate)
                        .ToList();
                }
                catch (Exception ex)
                {
                    this.Log.DebugFormat($"GetGroup LINQ custom method throws exception: {ex.Message}.");
                    throw;
                }

                return result;
            }
        }

        protected override void ExtenstionDispose() => this.Uow?.Dispose();
    }
}