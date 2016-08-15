using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;

namespace NaftanRailway.Domain.Abstract {
    /// <summary>
    /// This interface use for work with data DB (to select data in ORC and Sopod)
    /// </summary>
    public interface IBussinesEngage : IDisposable {
        bool DeleteInvoice(DateTime reportPeriod, Nullable<int> idInvoice);
        bool UpdateExists(DateTime reportPeriod);
        IEnumerable<Shipping> ShippingsViews(EnumOperationType operationCategory, DateTime chooseDate, int page, int pageSize, out short recordCount);
        /// <summary>
        /// Get current avaible type of operation on dispatch
        /// </summary>
        /// <returns></returns>
        List<short> GetTypeOfOpers(DateTime chooseDate);
        /// <summary>
        /// Sync time between session storage and menu on the page (this done for reload and ajax sync) =>correct sql =>return actual data
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="menuTime"></param>
        /// <returns></returns>
        DateTime SyncActualDate(SessionStorage storage, DateTime menuTime);
        ///long ShippingsViewsCount(string templShNumber, EnumOperationType operationCategory, DateTime chooseDate, byte shiftPage = 3);
        IEnumerable<string> AutoCompleteShipping(string templShNumber, DateTime chooseDate, byte shiftPage = 3);
        //IDictionary<short, int> Badges(string templShNumber, DateTime chooseDate, EnumOperationType operationCategory, byte shiftPage = 3);
        /// <summary>
        /// Get All info 
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <param name="preview"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        bool PackDocuments(DateTime reportPeriod, IList<ShippingInfoLine> preview, byte shiftPage = 5);

        bool PackDocSQL(DateTime reportPeriod, IList<ShippingInfoLine> preview, byte shiftPage = 3);
        /// <summary>
        /// Get general shipping info (v_otpr + v_o_v + etsng (mesplan)
        /// </summary>
        IEnumerable<ShippingInfoLine> ShippingPreview(string deliveryNote, DateTime dateOper, out short recordCount);

        /// <summary>
        /// Get rows from table (filter & order)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderPredicate"></param>
        /// <param name="caсhe"></param>
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
        /// <param name="orderPredicate">predicate for order</param>
        /// <param name="filterPredicate">predicate for filter result</param>
        /// <param name="page">current page</param>
        /// <param name="size">page Model[indx] size</param>
        /// <param name="caсhe"></param>
        /// <returns></returns>
        IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class;

        /// <summary>
        /// Get group result (Group by + order by)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groupPredicate"></param>
        /// <param name="predicate"></param>
        /// <param name="caсhe"></param>
        /// <returns></returns>
        IEnumerable<TKey> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> predicate = null, bool caсhe = false) where T : class;
        bool AddKrtNaftan(long key, out string msgError);
        /// <summary>
        /// Change Reporting date
        /// </summary>
        /// <param name="period"></param>
        /// <param name="key"></param>
        /// <param name="multiChange"></param>
        /// <returns></returns>
        bool ChangeBuhDate(DateTime period, long key, bool multiChange = true);
        bool EditKrtNaftanOrcSapod(long keykrt, long keysbor, decimal nds, decimal summa);
    }
}
