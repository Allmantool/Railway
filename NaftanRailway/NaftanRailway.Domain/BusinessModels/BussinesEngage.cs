using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Abstract;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// Класс отвечающий за формирование безнесс объектов (содержащий бизнес логику приложения)
    /// </summary>
    public class BussinesEngage {
        private readonly int _pageSize;
        private readonly int _shiftDay;

        public BussinesEngage(int pageSize, int shiftDay) {
            _pageSize = pageSize;
            _shiftDay = shiftDay;
        }

        /// <summary>
        /// Формирования объекта отображения информации об отправках
        /// </summary>
        /// <param name="documentRepository">Document Repository</param>
        /// <param name="templShNumber">Regular expression for searched filter</param>
        /// <param name="operationCategory">filter on category</param>
        /// <param name="chooseDate">Work period</param>
        /// <param name="page">Current page</param>
        /// <returns></returns>
        public IEnumerable<Shipping> ShippingsViews(IDocumentsRepository documentRepository, string templShNumber,
                EnumOperationType operationCategory, DateTime chooseDate, int page) {
            DateTime startDate = chooseDate.AddDays(_shiftDay);
            DateTime endDate = chooseDate.AddMonths(1).AddDays(_shiftDay);

            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together)
            var srcEntsg = documentRepository.Etsngs;

            var srcShipping = documentRepository.ShippinNumbers.Where(sh => sh.n_otpr.StartsWith(templShNumber) &&
                                (operationCategory == EnumOperationType.All || sh.oper == (short)operationCategory) && 
                                (sh.date_oper >= startDate && sh.date_oper <= endDate))
                                .OrderByDescending(sh => sh.date_oper)
                                .Skip((page - 1) * _pageSize)
                                .Take(_pageSize)
                                .ToList();

            return (from itemSh in srcShipping join itemEtsng in srcEntsg on itemSh.cod_tvk_etsng equals itemEtsng.etsng1
                    into gResult
                    from leftJoinResult in gResult.DefaultIfEmpty()
                    select new Shipping() {
                        VOtpr = itemSh,
                        Etsng = gResult,
                        Vov = documentRepository.CarriageNumbers.Where(cr => cr.id_otpr==itemSh.id)
                    }).AsEnumerable();
        }

        /// <summary>
        /// Count items for pagging
        /// </summary>
        /// <param name="documentRepository"></param>
        /// <param name="templShNumber"></param>
        /// <param name="operationCategory"></param>
        /// <param name="chooseDate"></param>
        /// <returns></returns>
        public int ShippingsViewsCount(IDocumentsRepository documentRepository, string templShNumber,
            EnumOperationType operationCategory, DateTime chooseDate) {

            DateTime startDate = chooseDate.AddDays(_shiftDay);
            DateTime endDate = chooseDate.AddMonths(1).AddDays(_shiftDay);

            return (documentRepository.ShippinNumbers).Count(sh => sh.n_otpr.StartsWith(templShNumber) &&
                        (operationCategory == EnumOperationType.All || sh.oper == (short)operationCategory) && 
                        (sh.date_oper >= startDate && sh.date_oper <= endDate));
        }
    }
}


