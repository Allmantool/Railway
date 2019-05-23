namespace NaftanRailway.BLL.Abstract
{
    using log4net;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Railway.Core.Data.EF;

    public interface IBusinessProvider : IDisposable
    {
        UnitOfWork Uow { get; set; }

        ILog Log { get; }

        IEnumerable<T> GetTable<T, TKey>(
            Expression<Func<T, bool>> predicate = null,
            Expression<Func<T, TKey>> orderPredicate = null,
            bool cache = false,
            bool tracking = false)
            where T : class;

        long GetCountRows<T>(Expression<Func<T, bool>> predicate = null)
            where T : class;

        IEnumerable<T> GetSkipRows<T, TKey>(
            int page,
            int size,
            Expression<Func<T, TKey>> orderPredicate,
            Expression<Func<T, bool>> filterPredicate = null,
            bool cache = false)
            where T : class;

        IEnumerable<IGrouping<TKey, T>> GetGroup<T, TKey>(
            Expression<Func<T, TKey>> groupPredicate,
            Expression<Func<T, bool>> filterPredicate = null,
            Expression<Func<T, TKey>> orderPredicate = null,
            bool cache = false)
            where T : class;
    }
}