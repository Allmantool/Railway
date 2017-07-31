using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NaftanRailway.Domain.Abstract;
using System.Linq;
using log4net;

namespace NaftanRailway.BLL.Abstract {
    /// <summary>
    /// This interface use for work with data DB (to select data in ORC and Sopod)
    /// </summary>
    public interface IBussinesEngage : IDisposable {
        IUnitOfWork Uow { get; set; }
        ILog Log { get; }

        /// <summary>
        /// Get rows from table (filter & order)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderPredicate"></param>
        /// <param name="caсhe"></param>
        /// <param name="tracking"></param>
        /// <returns></returns>
        IEnumerable<T> GetTable<T, TKey>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TKey>> orderPredicate = null, bool caсhe = false, bool tracking = false) where T : class;

        /// <summary>
        /// Return rows count of current row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        long GetCountRows<T>(Expression<Func<T, bool>> predicate = null) where T : class;

        /// <summary>
        /// I dont find method work with Expression three and func (TDelegate). 
        /// I want pass diffrent intergated function (OrderBy,Skip,Take etc ...) in body Expression three
        /// Now i add some addtional method in Bussiness class 
        /// (Requered some addional work!) Linq => expression three 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="recordCount"></param>
        /// <param name="orderPredicate">predicate for order</param>
        /// <param name="filterPredicate">predicate for filter result</param>
        /// <param name="page">current page</param>
        /// <param name="size">page Model[indx] size</param>
        /// <param name="caсhe"></param>
        /// <returns></returns>
        IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, out long recordCount, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class;
        IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class;

        /// <summary>
        /// Get group result (Group by + where + order by ... asc)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groupPredicate"></param>
        /// <param name="predicate"></param>
        /// <param name="caсhe"></param>
        /// <returns></returns>
        IEnumerable<IGrouping<TKey, T>> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> filterPredicate = null, Expression<Func<T, TKey>> orderPredicate = null, bool caсhe = false) where T : class;
    }
}