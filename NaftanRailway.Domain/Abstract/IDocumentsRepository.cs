using System;
using System.Linq;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;

namespace NaftanRailway.Domain.Abstract {
    /// <summary>
    /// This interface use for work with data DB (to select data in ORC and Sopod)
    /// </summary>
    public interface IDocumentsRepository:IDisposable {
        /// <summary>
        /// Get All info 
        /// </summary>
        /// <param name="shipping">requered shipping number</param>
        /// <param name="warehouse">correcting warehouse</param>
        /// <returns></returns>
        ShippingInfoLine PackDocuments(v_otpr shipping,int warehouse);
        /// <summary>
        /// Get general shipping info (v_otpr + v_o_v + etsng (mesplan)
        /// </summary>
        IQueryable<Shipping> ShippingInformation { get; }
        /// <summary>
        /// Get Shipping info
        /// </summary>
        IQueryable<v_otpr> ShippinNumbers { get; }
        /// <summary>
        /// Get info abount wagons
        /// </summary>
        IQueryable<v_o_v> CarriageNumbers { get; }
        /// <summary>
        /// Get info about Accumulative Cards
        /// </summary>
        IQueryable <AccumulativeCard> Cards { get; }
        /// <summary>
        /// Get luggage
        /// </summary>
        IQueryable<Luggage> Baggage { get; }

        void AddKrtNaftan(krt_Naftan record);
    }
}
