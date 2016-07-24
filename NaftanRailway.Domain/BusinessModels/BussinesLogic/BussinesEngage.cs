using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.Domain.ExpressionTreeExtensions;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    /// <summary>
    /// Класс отвечающий за формирование безнесс объектов (содержащий бизнес логику приложения)
    /// </summary>
    public class BussinesEngage : IBussinesEngage {
        private bool _disposed;
        private IUnitOfWork Uow { get; set; }
        public BussinesEngage(IUnitOfWork unitOfWork) {
            Uow = unitOfWork;
            _disposed = false;
        }

        /// <summary>   
        /// Формирования объекта отображения информации об отправках (по накладной за отчётный месяц)
        /// </summary>
        /// <param name="operationCategory">filter on category</param>
        /// <param name="chooseDate">Work period</param>
        /// <param name="page">Current page</param>
        /// <param name="pageSize">Count item on page</param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IEnumerable<Shipping> ShippingsViews(EnumOperationType operationCategory, DateTime chooseDate, int page, int pageSize, out short recordCount) {
            //exit when empty result (discrease count server query)
            if (GetCountRows<krt_Guild18>(x => x.reportPeriod == chooseDate) == 0) {
                recordCount = 0;
                return new List<Shipping>();
            }
            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together, resolve through expression tree maybe)
            var wrkData = GetTable<krt_Guild18, int>(x => x.reportPeriod == chooseDate).ToList();
            //dispatch
            var kg18Src = wrkData.GroupBy(x => new { x.reportPeriod, x.idDeliviryNote, x.warehouse })
                .OrderBy(x => x.Key.idDeliviryNote)
                .Skip(pageSize * (page - 1))
                .Take(pageSize).ToList();
            /*linqkit*/
            //v_otpr
            var votprPredicate = PredicateBuilder.False<v_otpr>().And(x => x.state == 32 && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol)));
            votprPredicate = kg18Src.Select(x => x.Key.idDeliviryNote).Aggregate(votprPredicate, (current, value) => current.Or(e => e.id == value)).Expand();
            var voSrc = GetTable<v_otpr, int>(votprPredicate).ToList();
            //v_o_v
            var vovPredicate = PredicateBuilder.False<v_o_v>();
            vovPredicate = voSrc.Select(x => x.id).Aggregate(vovPredicate, (current, value) => current.Or(v => v.id_otpr == value)).Expand();
            var vovSrc = GetTable<v_o_v, int>(vovPredicate).ToList();
            //etsng
            var etsngPredicate = PredicateBuilder.False<etsng>();
            etsngPredicate = voSrc.Select(x => x.cod_tvk_etsng).Aggregate(etsngPredicate, (current, value) => current.Or(v => v.etsng1 == value)).Expand();
            var etsngSrc = GetTable<etsng, int>(etsngPredicate).ToList();

            var result = (from kg in kg18Src join vo in voSrc on kg.Key.idDeliviryNote equals vo.id into g1
                          from item in g1.DefaultIfEmpty() where (item != null && item.oper == (short)operationCategory) || operationCategory == EnumOperationType.All
                          join e in etsngSrc on item == null ? "" : item.cod_tvk_etsng equals e.etsng1 into g2
                          from item2 in g2.DefaultIfEmpty()
                          select new Shipping() {
                              VOtpr = item,
                              Vovs = vovSrc.Where(x => (x != null) && x.id_otpr == item.id),
                              VPams = GetTable<v_pam, int>(PredicateBuilder.True<v_pam>().And(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_pam, int>("id_ved",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 2).Select(y => (int)y.idSrcDocument))).Expand())
                                .ToList(),
                              VAkts = GetTable<v_akt, int>(PredicateBuilder.True<v_akt>().And(x => new[] { "3494", "349402" }.Contains(x.kodkl) && x.state == 32)
                                .And(PredicateExtensions.InnerContainsPredicate<v_akt, int>("id",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 3).Select(y => (int)y.idSrcDocument))).Expand())
                                .ToList(),
                              VKarts = GetTable<v_kart, int>(PredicateBuilder.True<v_kart>().And(x => new[] { "3494", "349402" }.Contains(x.cod_pl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_kart, int>("id",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idCard ?? 0))).Expand())
                                .ToList(),
                              KNaftan = GetTable<krt_Naftan, int>(PredicateExtensions.InnerContainsPredicate<krt_Naftan, long>("keykrt",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => (long)y.idScroll)))
                                .ToList(),
                              Etsng = item2,
                              Guild18 = new krt_Guild18 {
                                  reportPeriod = kg.Key.reportPeriod,
                                  idDeliviryNote = kg.Key.idDeliviryNote,
                                  warehouse = kg.Key.warehouse
                              }
                          }).ToList();

            recordCount = (short)result.Count();
            return result.ToList();
        }
        /// <summary>
        /// Get current avaible type of operation on dispatch
        /// </summary>
        /// <param name="chooseDate"></param>
        /// <returns></returns>
        public List<short> GetTypeOfOpers(DateTime chooseDate) {
            var wrkDispatch = GetGroup<krt_Guild18, int>(x => (int)x.idDeliviryNote, x => x.reportPeriod == chooseDate && x.idDeliviryNote != null).ToList();
            var result = GetGroup<v_otpr, short>(x => (short)x.oper,
                x => x.state == 32 && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol)) && wrkDispatch.Contains(x.id))
                .ToList();

            return result;
        }
        public DateTime SyncActualDate(SessionStorage storage, DateTime menuTime) {
            //reload page (save select report date)
            if (storage.ReportPeriod == DateTime.MinValue && menuTime == DateTime.MinValue) {
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            if (storage.ReportPeriod != DateTime.MinValue && menuTime == DateTime.MinValue) {
                return storage.ReportPeriod;
            }

            storage.ReportPeriod = menuTime;

            return menuTime;
        }
        /// <summary>
        /// Autocomplete function
        /// </summary>
        /// <param name="templShNumber"></param>
        /// <param name="chooseDate"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public IEnumerable<string> AutoCompleteShipping(string templShNumber, DateTime chooseDate, byte shiftPage = 3) {
            var startDate = chooseDate.AddDays(-shiftPage);
            var endDate = chooseDate.AddMonths(1).AddDays(shiftPage);

            return GetGroup<v_otpr, string>(x => new { x.n_otpr }.n_otpr, x => x.n_otpr.StartsWith(templShNumber)
                 && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))
                 && x.state == 32 && (x.date_oper >= startDate && x.date_oper <= endDate))
                .OrderByDescending(x => x).Take(10);
        }
        public bool PackDocuments(DateTime reportPeriod, IList<ShippingInfoLine> preview, byte shiftPage = 3) {
            var startDate = reportPeriod.AddDays(-shiftPage);
            var endDate = reportPeriod.AddMonths(1).AddDays(shiftPage);
            List<krt_Guild18> result;

            try {
                //type_doc 1 => one trunsaction (one request per one dbcontext)
                result = (from item in preview join vn in GetTable<v_nach, int>(
                                  PredicateBuilder.True<v_nach>().And(x => x.type_doc == 1 && new[] { "3494", "349402" }.Contains(x.cod_kl))
                                      .And(PredicateExtensions.InnerContainsPredicate<v_nach, int?>("id_otpr", preview.Select(x => (int?)x.Shipping.id))).Expand())
                              on item.Shipping.id equals vn.id_otpr
                          select new krt_Guild18() {
                              reportPeriod = reportPeriod,
                              warehouse = item.Warehouse,
                              idDeliviryNote = item.Shipping.id,
                              type_doc = 1, idSrcDocument = item.Shipping.id,
                              code = Convert.ToInt32(vn.cod_sbor.Split(new[] { '.', ',' })[0]),
                              sum = (decimal)(vn.summa + vn.nds),
                              rateVAT = Math.Round((decimal)(vn.nds / vn.summa), 2),
                              codeType = new[] { 166, 173, 300, 301, 344 }.Contains(Convert.ToInt32(vn.cod_sbor.Split(new[] { '.', ',' })[0])),
                              idCard = vn.id_kart,
                              idScroll = GetGroup<krt_Naftan_orc_sapod, long?>(x => x.keykrt, x => x.id_kart == vn.id_kart).FirstOrDefault()
                          }).ToList();

                foreach (var dispatch in preview) {
                    var shNumbers = dispatch.WagonsNumbers.Select(x => x.n_vag).ToList();
                    //type_doc 2 =>one trunsaction (one request per one dbcontext) (type 2 and type_doc 4 (065))
                    //in memory because not all method support entity to sql => more easy do it in memory
                    using (Uow = new UnitOfWork()) {
                        result.AddRange((from vpv in Uow.Repository<v_pam_vag>().Get_all(x => shNumbers.Contains(x.nomvag), false)
                                         join vp in Uow.Repository<v_pam>().Get_all(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl) && x.dved > startDate && x.dved < endDate, false) on vpv.id_ved equals vp.id_ved
                                         join vn in Uow.Repository<v_nach>().Get_all(x => x.type_doc == 2 && new[] { "3494", "349402" }.Contains(x.cod_kl), false)
                                             on vp.id_kart equals vn.id_kart
                                         select new { vp.id_ved, vn.cod_sbor, vn.summa, vn.nds, vn.id_kart }).Distinct().ToList()
                            .Select(x => new krt_Guild18 {
                                reportPeriod = reportPeriod,
                                warehouse = dispatch.Warehouse,
                                idDeliviryNote = dispatch.Shipping.id,
                                type_doc = 2, idSrcDocument = x.id_ved,
                                code = Convert.ToInt32(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                sum = (decimal)(x.summa + x.nds),
                                idCard = x.id_kart, rateVAT = Math.Round((decimal)(x.nds / x.summa), 2),
                                codeType = new[] { "166", "173", "300", "301", "344" }.Contains(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                idScroll = GetGroup<krt_Naftan_orc_sapod, long?>(y => y.keykrt, z => z.id_kart == x.id_kart).FirstOrDefault()
                            }));
                    }
                    //065
                    using (Uow = new UnitOfWork()) {
                        result.AddRange((from vpv in Uow.Repository<v_pam_vag>().Get_all(x => shNumbers.Contains(x.nomvag), false) join vn in Uow.Repository<v_nach>()
                                         .Get_all(x => x.type_doc == 4 && x.cod_sbor == "065" && new[] { "3494", "349402" }.Contains(x.cod_kl) && x.date_raskr > startDate && x.date_raskr < endDate, false) on
                                             new { p1 = vpv.d_pod, p2 = vpv.d_ub } equals new { p1 = vn.date_raskr, p2 = vn.date_raskr }
                                         select new { vpv.id_ved, vn.cod_sbor, vn.summa, vn.nds, vn.id_kart }).Distinct().ToList()
                            .Select(x =>
                                new krt_Guild18 {
                                    reportPeriod = reportPeriod,
                                    warehouse = dispatch.Warehouse,
                                    idDeliviryNote = dispatch.Shipping.id,
                                    type_doc = 4, idSrcDocument = x.id_kart,
                                    code = Convert.ToInt32(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    sum = (decimal)(x.summa + x.nds),
                                    idCard = x.id_kart, rateVAT = Math.Round((decimal)(x.nds / x.summa), 2),
                                    codeType = new[] { "166", "173", "300", "301", "344" }.Contains(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    idScroll = GetGroup<krt_Naftan_orc_sapod, long?>(y => y.keykrt, z => z.id_kart == x.id_kart).FirstOrDefault()
                                }));
                    }
                    ////type_doc 3 =>one trunsaction (one request per one dbcontext)
                    using (Uow = new UnitOfWork()) {
                        result.AddRange((from vav in Uow.Repository<v_akt_vag>().Get_all(x => shNumbers.Contains(x.nomvag), false)
                                         join va in Uow.Repository<v_akt>().Get_all(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl) && x.dakt > startDate && x.dakt < endDate, false) on vav.id_akt equals va.id
                                         join vn in Uow.Repository<v_nach>().Get_all(x => x.type_doc == 3 && new[] { "3494", "349402" }.Contains(x.cod_kl), false)
                                             on va.id_kart equals vn.id_kart
                                         select new { va.id, vn.cod_sbor, vn.summa, vn.nds, vn.id_kart }).Distinct().ToList()
                            .Select(x =>
                                new krt_Guild18 {
                                    reportPeriod = reportPeriod,
                                    warehouse = dispatch.Warehouse,
                                    idDeliviryNote = dispatch.Shipping.id,
                                    type_doc = 3, idSrcDocument = x.id,
                                    code = Convert.ToInt32(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    sum = (decimal)(x.summa + x.nds),
                                    idCard = x.id_kart,
                                    rateVAT = Math.Round((decimal)(x.nds / x.summa), 2),
                                    codeType = new[] { "166", "173", "300", "301", "344" }.Contains(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    idScroll = GetGroup<krt_Naftan_orc_sapod, long?>(y => y.keykrt, z => z.id_kart == x.id_kart).FirstOrDefault()
                                }));

                    }
                    //luggage (type_doc 0 or 4)
                    result.AddRange(GetTable<krt_Naftan_orc_sapod, long>(x => new[] { 611, 629, 125 }.Contains(x.vidsbr) && x.dt.Month == reportPeriod.Month && x.dt.Year == reportPeriod.Year).ToList()
                        .Select(x => new krt_Guild18 {
                            reportPeriod = reportPeriod,
                            type_doc = x.tdoc,
                            idSrcDocument = x.id_kart, code = x.vidsbr,
                            sum = x.sm, rateVAT = Math.Round((decimal)(x.nds / x.sm_no_nds), 2),
                            codeType = new[] { 166, 173, 300, 301, 344 }.Contains(x.vidsbr),
                            idCard = x.id_kart, idScroll = x.keykrt
                        }));
                }
            } catch (Exception) {
                return false;
            }
            //add/update
            using (Uow = new UnitOfWork()) {
                var groupInvoce = result.GroupBy(x => new { x.reportPeriod, x.idDeliviryNote }).Select(x => new { x.Key.reportPeriod, x.Key.idDeliviryNote });
                //circle 1 per 1 cilcle invoice
                foreach (var invoce in groupInvoce) {
                    var temp = invoce;
                    //if exist some information => delete , because we don't have appreate primary key for merge/Update operations
                    Uow.Repository<krt_Guild18>().Delete(x => x.reportPeriod == temp.reportPeriod && x.idDeliviryNote == temp.idDeliviryNote, false);

                    //add information about 1 per 1 cicle invoice
                    Uow.Repository<krt_Guild18>().AddRange(result.Where(x => x.reportPeriod == temp.reportPeriod && x.idDeliviryNote == temp.idDeliviryNote), false);
                }
                Uow.Save();
            }
            return true;
        }
        /// <summary>
        /// Альтернатива PAckDocuments. Т.к EF6  не поддерживает работу Join с двумя котестами (таблицы находяться на разных серверах)=>
        /// Возможные варианты (выбран 3)
        /// 1)Создание view для таблиц из другого сервера 
        /// 2)ковыряние в edmx (необходимо также работа c synonem в  SQL)
        /// 3)Написание SQL прямых заросов и разметка сущностей (теряем обстракцию, т.к необходимо указывать явные имена linkid servers)
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <param name="preview"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public bool PackDocSQL(DateTime reportPeriod, IList<ShippingInfoLine> preview, byte shiftPage = 3) {
            var startDate = reportPeriod.AddDays(-shiftPage).Date;
            var endDate = reportPeriod.AddMonths(1).AddDays(shiftPage).Date;

            var result = new List<krt_Guild18>();

            foreach (var dispatch in preview) {
                var temp = dispatch;

                using (Uow = new UnitOfWork()) {
                    //для динамического соединения
                    var sapodConn = "[" + Uow.Repository<v_otpr>().Context.Database.Connection.DataSource + @"].[" + Uow.Repository<v_otpr>().Context.Database.Connection.Database + @"]";
                    var orcConn = "[" + Uow.Repository<krt_Guild18>().Context.Database.Connection.DataSource + @"].[" + Uow.Repository<krt_Guild18>().Context.Database.Connection.Database + @"]";
                    var carriages = (temp.WagonsNumbers.Any()) ? string.Join(",", temp.WagonsNumbers.Select(x => string.Format("'{0}'", x.n_vag))) : string.Empty;

                    //Для mapping требуется точное совпадение имен и типов столбцов
                    //Выбираем с какой стороны работать (сервер) по сущности
                    result.AddRange(Uow.Repository<krt_Guild18>().Context.Database.SqlQuery<krt_Guild18>(@"
                        SELECT 0 as [id], @reportPeriod AS [reportPeriod],@warehouse AS [warehouse],vo.id AS [idDeliviryNote],knos.tdoc AS [type_doc],vo.id AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN knos.vidsbr IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],CONVERT(int,knos.vidsbr) AS [code],knos.sm as [sum],
                            CONVERT(decimal(3,2),knos.stnds/100) as [rateVAT],keykrt AS [idScroll],knos.id_kart AS [idCard]
                        FROM " + sapodConn + @".[dbo].[v_otpr] as vo 
                            INNER JOIN " + orcConn + @".[dbo].[krt_Naftan_orc_sapod] AS knos ON knos.id_otpr = vo.id 
                        WHERE vo.[state] = 32 AND (vo.cod_kl_otpr in ('3494','349402') OR vo.cod_klient_pol in ('3494','349402')) AND knos.tdoc = 1 AND vo.id = @id_otpr
                        UNION ALL
                        SELECT distinct 0 as [id], @reportPeriod AS [reportPeriod], @warehouse AS [warehouse], @id_otpr AS [idDeliviryNote], knos.tdoc AS[type_doc],
                            CASE knos.tdoc WHEN 2 then vp.id_ved ELSE knos.id_kart end AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN knos.vidsbr IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType], CONVERT(int,knos.vidsbr) AS [code], knos.sm as [sum],
                            CONVERT(decimal(3, 2), knos.stnds / 100) as [rateVAT], knos.keykrt AS [idScroll], knos.id_kart AS [idCard]
                        FROM " + sapodConn + @".[dbo].v_pam as vp INNER JOIN " + sapodConn + @".[dbo].v_pam_vag AS vpv
                            ON vpv.id_ved = vp.id_ved LEFT JOIN " + orcConn + @".[dbo].[krt_Naftan_orc_sapod] AS knos
                                ON (knos.id_kart = vp.id_kart and knos.tdoc = 2) OR
                           (date_raskr IN (CONVERT(date, vpv.d_pod),CONVERT(date, vpv.d_ub)) AND knos.vidsbr = 65 AND knos.tdoc = 4)
                        WHERE vpv.nomvag IN (" + carriages + @") AND vp.kodkl in ('3494','349402') AND [state] = 32 AND (vp.dved BETWEEN @stDate AND @endDate)
                        UNION ALL
                        SELECT 0 as [id], @reportPeriod AS [reportPeriod], @warehouse AS [warehouse], @id_otpr AS [idDeliviryNote], knos.tdoc AS [type_doc], va.id AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN knos.vidsbr IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType], CONVERT(int,knos.vidsbr) AS [code], 
                            knos.sm AS [sum], CONVERT(decimal(3, 2), knos.stnds / 100) AS [rateVAT], keykrt AS [idScroll], knos.id_kart AS [idCard]
                        FROM " + sapodConn + @".[dbo].v_akt as va LEFT JOIN " + orcConn + @".[dbo].[krt_Naftan_orc_sapod] AS knos
                                ON knos.id_kart = va.id_kart
                        WHERE Exists (SELECT * from " + sapodConn + @".[dbo].v_akt_vag as vav where vav.nomvag IN (" + carriages + @") and va.id = vav.id_akt ) AND [state] = 32 AND knos.tdoc = 3 AND (va.dakt BETWEEN @stDate AND @endDate)
                        UNION ALL
                        SELECT 0 AS [id], @reportPeriod AS [reportPeriod],NULL AS [warehouse],NULL AS [idDeliviryNote],knos.tdoc AS [type_doc],
                            CASE knos.tdoc when 4 then knos.id_kart ELSE null END AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN knos.vidsbr IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType], CONVERT(int,knos.vidsbr) AS [code], 
                            knos.sm AS [sum], CONVERT(decimal(3, 2), knos.stnds / 100) AS [rateVAT], keykrt AS [idScroll], knos.id_kart AS [idCard]
                        FROM " + orcConn + @".[dbo].krt_Naftan_orc_sapod AS knos
                        WHERE knos.vidsbr IN (611, 629, 125) AND knos.dt BETWEEN @stDate AND @endDate;",
                        new SqlParameter("@reportPeriod", reportPeriod),
                        new SqlParameter("@warehouse", temp.Warehouse),
                        new SqlParameter("@id_otpr", (temp.Shipping == null) ? 0 : temp.Shipping.id),
                        new SqlParameter("@stDate", startDate),
                        new SqlParameter("@endDate", endDate)));
                }
            }

            //add/update
            using (Uow = new UnitOfWork()) {
                var groupInvoce = result.GroupBy(x => new { x.reportPeriod, x.idDeliviryNote }).Select(x => new { x.Key.reportPeriod, x.Key.idDeliviryNote });
                //circle 1 per 1 cilcle invoice
                foreach (var invoce in groupInvoce) {
                    var temp = invoce;
                    //if exist some information => delete , because we don't have appreate primary key for merge/Update operations
                    Uow.Repository<krt_Guild18>().Delete(x => x.reportPeriod == temp.reportPeriod && x.idDeliviryNote == temp.idDeliviryNote, false);
                    //add information about 1 per 1 cicle invoice
                    Uow.Repository<krt_Guild18>().AddRange(result.Where(x => x.reportPeriod == temp.reportPeriod && x.idDeliviryNote == temp.idDeliviryNote), false);
                    Uow.Save();
                }

            }
            return true;
        }
        public bool DeleteInvoice(DateTime reportPeriod, int idInvoice) {
            using (Uow = new UnitOfWork()) {
                try {
                    if (idInvoice == 0) {
                        //Доб. сборы
                        Uow.Repository<krt_Guild18>().Delete(x => x.reportPeriod == reportPeriod && x.idDeliviryNote == null, false);
                    } else { Uow.Repository<krt_Guild18>().Delete(x => x.reportPeriod == reportPeriod && x.idDeliviryNote == idInvoice, false); }

                } catch (Exception) {
                    return false;
                }
                Uow.Save();
            }
            return true;
        }
        /// <summary>
        /// Update existing information in definite period time
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <returns></returns>
        public bool UpdateExists(DateTime reportPeriod) {
            var dataRows = GetTable<krt_Guild18, int?>(x => x.reportPeriod == reportPeriod && x.idDeliviryNote != null).GroupBy(x => new { x.idDeliviryNote, x.warehouse })
                .Select(y => new ShippingInfoLine() {
                    Shipping = GetTable<v_otpr, int>(x => x.id == y.Key.idDeliviryNote).First(),
                    WagonsNumbers = GetTable<v_o_v, int>(x => x.id_otpr == y.Key.idDeliviryNote).ToList(),
                    Warehouse = (int)y.Key.warehouse
                }).ToList();

            PackDocSQL(reportPeriod, dataRows);

            return true;
        }
        public IEnumerable<ShippingInfoLine> ShippingPreview(string deliveryNote, DateTime dateOper, out short recordCount) {
            DateTime startDate = dateOper.AddDays(-3);
            DateTime endDate = dateOper.AddMonths(1).AddDays(3);
            IEnumerable<ShippingInfoLine> result = new List<ShippingInfoLine>();

            var delivery = GetTable<v_otpr, int>(x => x.n_otpr == deliveryNote && x.state == 32 && x.date_oper >= startDate && x.date_oper <= endDate &&
               (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))).ToList();

            if (!delivery.Any()) {
                recordCount = 0;
            } else {
                //one trunsaction (one request per one dbcontext)
                using (Uow = new UnitOfWork()) {
                    result = (from sh in delivery join e in Uow.Repository<etsng>().Get_all(enablecaching: false) on sh.cod_tvk_etsng equals e.etsng1
                              select new ShippingInfoLine() {
                                  Shipping = sh,
                                  CargoEtsngName = e,
                                  WagonsNumbers = GetTable<v_o_v, int>(x => x.id_otpr == sh.id).ToList(),
                              }).ToList();
                }
                recordCount = (short)result.Count();
            }
            return result;
        }
        /// <summary>
        /// Get General table with predicate ( load in memory)
        /// </summary>
        public IEnumerable<T> GetTable<T, TKey>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TKey>> orderPredicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return (orderPredicate == null) ? Uow.Repository<T>().Get_all(predicate, caсhe).ToList() : Uow.Repository<T>().Get_all(predicate, caсhe).OrderByDescending(orderPredicate).ToList();
            }
        }
        public long GetCountRows<T>(Expression<Func<T, bool>> predicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate, caсhe).Count();
            }
        }
        /// <summary>
        /// Return pagging part of table
        /// </summary>
        /// <typeparam name="T">Current enity</typeparam>
        /// <typeparam name="TKey">Type for ordering</typeparam>
        /// <param name="page">Number page</param>
        /// <param name="size">Count row per one page</param>
        /// <param name="orderPredicate">Condition for ordering</param>
        /// <param name="filterPredicate">Condition for filtering</param>
        /// <returns>Return definition count rows of specific entity</returns>
        public IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(filterPredicate, caсhe).OrderByDescending(orderPredicate).Skip((page - 1) * size).Take(size).ToList();
            }
        }
        public IEnumerable<TKey> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> predicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate, caсhe).GroupBy(groupPredicate).OrderBy(x => x.Key).Select(x => x.Key).ToList();
            }
        }
        /// <summary>
        /// Operation adding information about scroll in table Krt_Naftan_Orc_Sapod and check operation as perfomed in krt_Naftan
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msgError"></param>
        public bool AddKrtNaftan(long key, out string msgError) {
            using (Uow = new UnitOfWork()) {
                try {
                    SqlParameter parm = new SqlParameter() {
                        ParameterName = "@ErrId",
                        SqlDbType = SqlDbType.TinyInt,
                        Direction = ParameterDirection.Output
                    };
                    //set active context => depend on type of entity
                    Uow.Repository<krt_Naftan_orc_sapod>().Context.Database.CommandTimeout = 120;
                    Uow.Repository<krt_Naftan_orc_sapod>().Context.Database.ExecuteSqlCommand(@"EXEC @ErrId = dbo.sp_fill_krt_Naftan_orc_sapod @KEYKRT", new SqlParameter("@KEYKRT", key), parm);
                    //alternative, get gropu of entities and then  save in db
                    //this.Database.SqlQuery<YourEntityType>("storedProcedureName",params);

                    //Confirmed
                    krt_Naftan chRecord = Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == key);

                    Uow.Repository<krt_Naftan>().Edit(chRecord);
                    if (!chRecord.Confirmed) {
                        chRecord.Confirmed = true;
                        chRecord.CounterVersion = 1;
                    }

                    chRecord.ErrorState = Convert.ToByte((byte)parm.Value);
                } catch (Exception e) {
                    msgError = e.Message;
                    return false;
                }

                Uow.Save();
            }
            msgError = "";
            return true;
        }
        /// <summary>
        /// Change date all later records
        /// </summary>
        /// <param name="period"></param>
        /// <param name="key"></param>
        /// <param name="multiChange">Change single or multi date</param>
        public bool ChangeBuhDate(DateTime period, long key, bool multiChange = true) {
            using (Uow = new UnitOfWork()) {
                var listRecords = multiChange ? Uow.Repository<krt_Naftan>().Get_all(x => x.KEYKRT >= key) : Uow.Repository<krt_Naftan>().Get_all(x => x.KEYKRT == key);
                try {
                    foreach (krt_Naftan item in listRecords.OrderByDescending(x => x.KEYKRT).ToList()) {
                        Uow.Repository<krt_Naftan>().Edit(item);
                        item.DTBUHOTCHET = period;
                    }
                    Uow.Save();
                    return true;
                } catch (Exception e) {
                    return false;
                }
            }
        }
        /// <summary>
        /// Edit Row (sm, sm_nds (Sapod))
        /// Check row as fix => check ErrorState in krt_Naftan_Sapod 
        /// </summary>
        /// <param name="keykrt">partial key (keykrt, keysbor)</param>
        /// <param name="keysbor">partial key (keykrt, keysbor)</param>
        /// <param name="nds"></param>
        /// <param name="summa"></param>
        /// <returns></returns>
        public bool EditKrtNaftanOrcSapod(long keykrt, long keysbor, decimal nds, decimal summa) {
            using (Uow = new UnitOfWork()) {
                try {
                    //krt_Naftan_ORC_Sapod (check as correction)
                    var itemRow = Uow.Repository<krt_Naftan_orc_sapod>().Get(x => x.keykrt == keykrt && x.keysbor == keysbor);
                    Uow.Repository<krt_Naftan_orc_sapod>().Edit(itemRow);
                    itemRow.nds = nds;
                    itemRow.summa = summa;
                    itemRow.ErrorState = 2;

                    //krt_Naftan (check as correction)
                    var parentRow = Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == keykrt);
                    Uow.Repository<krt_Naftan>().Edit(parentRow);

                    parentRow.ErrorState = 2;

                    Uow.Save();

                } catch (Exception e) {
                    return false;
                }

                return true;
            }
        }
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing)
                    Uow.Dispose();
            }
            _disposed = true;
        }
    }
}