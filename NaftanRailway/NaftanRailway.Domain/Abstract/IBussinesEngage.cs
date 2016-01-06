using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.Abstract {
    /// <summary>
    /// This interface use for work with data DB (to select data in ORC and Sopod)
    /// </summary>
    public interface IBussinesEngage{
        IEnumerable<Shipping> ShippingsViews(string templShNumber,EnumOperationType operationCategory, DateTime chooseDate, int page,int shiftDate , int pageSize);
        int ShippingsViewsCount(string templShNumber,EnumOperationType operationCategory, DateTime chooseDate,byte shiftPage = 3);
        IEnumerable<string> AutoCompleteShipping(string templShNumber,DateTime chooseDate,byte shiftPage = 3);
        Dictionary<short, int> Badges(string templShNumber,DateTime chooseDate,EnumOperationType operationCategory,byte shiftPage = 3);

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
        IQueryable <AccumulativeCard> Cards { get; }
        /// <summary>
        /// Get luggage
        /// </summary>
        IQueryable<Luggage> Baggage { get; }

        IQueryable<v_pam_vag> PamVags { get; }
        IQueryable<v_pam_sb> PamSbs {get;}
        IQueryable<v_pam> Pams { get; }
        IQueryable<v_akt> Akts { get; }
        IQueryable<v_akt_sb> AktSbs { get; }
        IQueryable<v_akt_vag> AktVags { get; }
        IQueryable<v_kart> Karts { get; }
        IQueryable<v_nach> Naches { get; }
        IQueryable<orc_krt> OrcKrts { get; }
        IQueryable<orc_sbor> OrcSbors { get; }
        IQueryable<etsng> Etsngs { get; }
        IQueryable<krt_Naftan> GetKrtNaftans { get; }
        void AddKrtNaftan(DateTime period, long key);
    }
}
