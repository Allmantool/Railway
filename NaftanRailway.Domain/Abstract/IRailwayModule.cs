using System;
using System.Collections.Generic;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.BusinessModels.SessionLogic;

namespace NaftanRailway.Domain.Abstract {
    public interface IRailwayModule : IDisposable {
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
        bool PackDocSql(DateTime reportPeriod, IList<ShippingInfoLine> preview, byte shiftPage = 3);
        /// <summary>
        /// Get general shipping info (v_otpr + v_o_v + etsng (mesplan)
        /// </summary>
        IEnumerable<ShippingInfoLine> ShippingPreview(string deliveryNote, DateTime dateOper, out short recordCount);
    }
}
