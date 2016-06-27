using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.Abstract {
    /// <summary>
    /// This interface use for work with data DB (to select data in ORC and Sopod)
    /// </summary>
    public interface IBussinesEngage : IDisposable {
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
        /// <param name="deliveryNote">correcting warehouse</param>
        /// <param name="recordCount">correcting warehouse</param>
        /// <returns></returns>
        ShippingInfoLine PackDocuments(string deliveryNote, out short recordCount);
        /// <summary>
        /// Get general shipping info (v_otpr + v_o_v + etsng (mesplan)
        /// </summary>
        IQueryable<Shipping> ShippingInformation { get; }
        /// <summary>
        /// Get Shipping info
        /// </summary>
        //IQueryable<v_otpr> ShippinNumbers { get; }
        /// <summary>
        /// Get info abount wagons
        /// </summary>
        IQueryable<v_o_v> CarriageNumbers { get; }
        /// <summary>
        /// Get bills info
        /// </summary>
        IQueryable<Bill> Bills { get; }
        /// <summary>
        /// Get info abount acts
        /// </summary>
        IQueryable<Certificate> Certificates { get; }
        /// <summary>
        /// Get info about Accumulative Cards
        /// </summary>
        IQueryable<AccumulativeCard> Cards { get; }
        /// <summary>
        /// Get luggage
        /// </summary>
        IQueryable<Luggage> Baggage { get; }
        IQueryable<v_pam_vag> PamVags { get; }
        IQueryable<v_pam_sb> PamSbs { get; }
        IQueryable<v_pam> Pams { get; }
        IQueryable<v_akt> Akts { get; }
        IQueryable<v_akt_sb> AktSbs { get; }
        IQueryable<v_akt_vag> AktVags { get; }
        IQueryable<v_kart> Karts { get; }
        IQueryable<v_nach> Naches { get; }
        IQueryable<orc_krt> OrcKrts { get; }
        IQueryable<orc_sbor> OrcSbors { get; }
        IQueryable<etsng> Etsngs { get; }
        /// <summary>
        /// Get rows from table (filter & order)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderPredicate"></param>
        /// <returns></returns>
        IEnumerable<T> GetTable<T, TKey>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TKey>> orderPredicate = null, bool caсhe = false) where T : class;
        /// <summary>
        /// Return rows count of current row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        long GetCountRows<T>(Expression<Func<T, bool>> predicate = null, bool caсhe = false) where T : class;
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
        /// <param name="size">page item size</param>
        /// <returns></returns>
        IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class;
        /// <summary>
        /// Get group result (Group by + order by)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="groupPredicate"></param>
        /// <param name="predicate"></param>
        /// <param name="orderPredicate"></param>
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
