using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    /// <summary>
    /// Класс отвечающий за формирование безнесс объектов (содержащий бизнес логику приложения)
    /// </summary>
    public class BussinesEngage : IBussinesEngage {
        private bool _disposed = false;
        private readonly UnitOfWork _unitOfWork;

        public BussinesEngage(UnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public BussinesEngage() {
          
        }

        /// <summary>
        /// Формирования объекта отображения информации об отправках
        /// </summary>
        /// <param name="templShNumber">Regular expression for searched filter</param>
        /// <param name="operationCategory">filter on category</param>
        /// <param name="chooseDate">Work period</param>
        /// <param name="page">Current page</param>
        /// <param name="shiftDate">correction number</param>
        /// <param name="pageSize">Count item on page</param>
        /// <returns></returns>
        public IEnumerable<Shipping> ShippingsViews(string templShNumber, EnumOperationType operationCategory,
            DateTime chooseDate, int page = 1, int shiftDate = 3, int pageSize = 8) {
            DateTime startDate = chooseDate.AddDays(shiftDate);
            DateTime endDate = chooseDate.AddMonths(1).AddDays(shiftDate);

            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together)
            var srcEntsg = _unitOfWork.Repository<etsng>().Get_all().ToList();

            var srcShipping =
                _unitOfWork.Repository<Shipping>()
                    .Get_all(sh => sh.VOtpr.n_otpr.StartsWith(templShNumber) &&
                        sh.VOtpr.state == 32 && ((new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_kl_otpr) && sh.VOtpr.oper == 1) ||
                        (new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_klient_pol) && sh.VOtpr.oper == 2)) &&
                        (operationCategory == EnumOperationType.All ||sh.VOtpr.oper == (short)operationCategory) &&
                        (sh.VOtpr.date_oper >= startDate &&sh.VOtpr.date_oper <= endDate))
                    .OrderByDescending(sh => sh.VOtpr.date_oper)
                    .Skip((page - 1)*pageSize)
                    .Take(pageSize)
                    .ToList();

            return (from itemSh in srcShipping
                    join itemEtsng in srcEntsg on itemSh.VOtpr.cod_tvk_etsng equals itemEtsng.etsng1
                        into gResult
                    from leftJoinResult in gResult.DefaultIfEmpty()
                    select new Shipping() {
                        VOtpr = itemSh.VOtpr,
                        Etsng = gResult,
                        Vov = _unitOfWork.Repository<v_o_v>().Get_all(cr => cr.id_otpr == itemSh.VOtpr.id)
                    }).AsEnumerable();
        }

        /// <summary>
        /// Count items for pagging
        /// </summary>
        /// <param name="templShNumber"></param>
        /// <param name="operationCategory"></param>
        /// <param name="chooseDate"></param>
        /// <param name="shiftDate"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public int ShippingsViewsCount(string templShNumber, EnumOperationType operationCategory, DateTime chooseDate, byte shiftPage = 3) {
            DateTime startDate = chooseDate.AddDays(-shiftPage);
            DateTime endDate = chooseDate.AddMonths(1).AddDays(shiftPage);

            return (_unitOfWork.Repository<Shipping>()
                .Get_all(sh => sh.VOtpr.state == 32 &&
                               ((new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_kl_otpr) && sh.VOtpr.oper == 1) ||
                                (new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_klient_pol) && sh.VOtpr.oper == 2)) &&
                               sh.VOtpr.n_otpr.StartsWith(templShNumber) &&
                               (operationCategory == EnumOperationType.All || sh.VOtpr.oper == (short)operationCategory) &&
                               (sh.VOtpr.date_oper >= startDate && sh.VOtpr.date_oper <= endDate)).Count());
        }

        /// <summary>
        /// Autocomplete function
        /// </summary>
        /// <param name="templShNumber"></param>
        /// <param name="chooseDate"></param>
        /// <param name="shiftDay"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public IEnumerable<string> AutoCompleteShipping(string templShNumber, DateTime chooseDate, byte shiftPage = 3) {
            return _unitOfWork.Repository<Shipping>().Get_all(sh => sh.VOtpr.state == 32 &&
                    ((new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_kl_otpr) && sh.VOtpr.oper == 1) ||
                     (new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_klient_pol) && sh.VOtpr.oper == 2)) &&
                    sh.VOtpr.n_otpr.StartsWith(templShNumber) &&
                    (sh.VOtpr.date_oper >= chooseDate.AddDays(-shiftPage) &&
                    sh.VOtpr.date_oper <= chooseDate.AddDays(shiftPage)))
                .GroupBy(g => new { g.VOtpr.n_otpr })
                .OrderByDescending(p => p.Key.n_otpr)
                .Select(m => m.Key.n_otpr)
                .Take(20).ToList();
        }

        public ShippingInfoLine PackDocuments(v_otpr shipping, int warehouse) {
            throw new NotImplementedException();
        }

        public IQueryable<Shipping> ShippingInformation {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_otpr> ShippinNumbers {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_o_v> CarriageNumbers {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<Bill> Bills {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<Certificate> Certificates {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<AccumulativeCard> Cards {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<Luggage> Baggage {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_pam_vag> PamVags {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_pam_sb> PamSbs {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_pam> Pams {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_akt> Akts {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_akt_sb> AktSbs {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_akt_vag> AktVags {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_kart> Karts {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_nach> Naches {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<orc_krt> OrcKrts {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<orc_sbor> OrcSbors {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<etsng> Etsngs {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<krt_Naftan> KrtNaftans {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<krt_Naftan_orc_sapod> KrtNaftanOrcSapods {
            get { throw new NotImplementedException(); }
        }

        public void AddKrtNaftan(krt_Naftan record) {
            throw new NotImplementedException();
        }

        public Dictionary<short, int> Badges(string templShNumber, DateTime chooseDate, EnumOperationType operationCategory, byte shiftPage = 3) {
            return _unitOfWork.Repository<Shipping>().Get_all(sh => sh.VOtpr.state == 32 &&
                    ((new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_kl_otpr) && sh.VOtpr.oper == 1) ||
                     (new[] { "3494", "349402" }.Contains(sh.VOtpr.cod_klient_pol) && sh.VOtpr.oper == 2)) && 
                     sh.VOtpr.n_otpr.StartsWith(templShNumber)
                            && (sh.VOtpr.date_oper >= chooseDate.AddDays(-shiftPage) && sh.VOtpr.date_oper <= chooseDate.AddMonths(1).AddDays(shiftPage))
                                && ((int)operationCategory == 0 || sh.VOtpr.oper == (int)operationCategory))
                     .GroupBy(x => new { x.VOtpr.oper })
                     .Select(g => new { g.Key.oper, operCount = g.Count() })
                     .ToDictionary(item => item.oper.Value, item => item.operCount);
        }

    }
}