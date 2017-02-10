using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using LinqKit;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.DTO.Guild18;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;
using NaftanRailway.BLL.Services.ExpressionTreeExtensions;

namespace NaftanRailway.BLL.Concrete.BussinesLogic {
    public class RailwayModule : Disposable, IRailwayModule {
        private readonly IBussinesEngage _engage;

        public RailwayModule(IBussinesEngage engage) {
            _engage = engage;
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
        public IEnumerable<ShippingDTO> ShippingsViews(EnumOperationType operationCategory, DateTime chooseDate, int page, int pageSize, out short recordCount) {
            //exit when empty result (discrease count server query)
            if (_engage.GetCountRows<krt_Guild18>(x => x.reportPeriod == chooseDate) == 0) {
                recordCount = 0;
                return new List<ShippingDTO>();
            }
            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together, resolve through expression tree maybe)
            var wrkData = _engage.GetTable<krt_Guild18, int>(x => x.reportPeriod == chooseDate).ToList();

            //dispatch
            var kg18Src = wrkData.GroupBy(x => new { x.reportPeriod, x.idDeliviryNote, x.warehouse })
                .OrderBy(x => x.Key.idDeliviryNote).ToList();

            /*linqkit*/
            //v_otpr
            var votprPredicate = PredicateBuilder.False<v_otpr>().And(x => ((x.oper == (short)operationCategory) || operationCategory == EnumOperationType.All) && x.state == 32 && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol)));
            votprPredicate = kg18Src.Select(x => x.Key.idDeliviryNote).Aggregate(votprPredicate, (current, value) => current.Or(e => e.id == value && ((e.oper == (short)operationCategory) || operationCategory == EnumOperationType.All))).Expand();
            var voSrc = _engage.GetTable<v_otpr, int>(votprPredicate).ToList();
            recordCount = (short)voSrc.Count();
            //v_o_v
            var vovPredicate = PredicateBuilder.False<v_o_v>();
            vovPredicate = voSrc.Select(x => x.id).Aggregate(vovPredicate, (current, value) => current.Or(v => v.id_otpr == value)).Expand();
            var vovSrc = _engage.GetTable<v_o_v, int>(vovPredicate).ToList();
            //etsng
            var etsngPredicate = PredicateBuilder.False<etsng>();
            etsngPredicate = voSrc.Select(x => x.cod_tvk_etsng).Aggregate(etsngPredicate, (current, value) => current.Or(v => v.etsng1 == value)).Expand();
            var etsngSrc = _engage.GetTable<etsng, int>(etsngPredicate).ToList();

            var result = (from kg in kg18Src join vo in voSrc on kg.Key.idDeliviryNote equals vo.id into g1
                          from item in g1.DefaultIfEmpty() where (item != null && item.oper == (short)operationCategory) || operationCategory == EnumOperationType.All
                          join e in etsngSrc on item == null ? "" : item.cod_tvk_etsng equals e.etsng1 into g2
                          from item2 in g2.DefaultIfEmpty()
                          select new ShippingDTO() {
                              VOtpr = item,
                              Vovs = vovSrc.Where(x => (x != null) && x.id_otpr == item.id),
                              VPams = _engage.GetTable<v_pam, int>(PredicateBuilder.True<v_pam>().And(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_pam, int>("id_ved",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 2).Select(y => y.idSrcDocument != null ? (int)y.idSrcDocument : 0))).Expand())
                                .ToList(),
                              VAkts = _engage.GetTable<v_akt, int>(PredicateBuilder.True<v_akt>().And(x => new[] { "3494", "349402" }.Contains(x.kodkl) && x.state == 32)
                                .And(PredicateExtensions.InnerContainsPredicate<v_akt, int>("id",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 3).Select(y => y.idSrcDocument != null ? (int)y.idSrcDocument : 0))).Expand())
                                .ToList(),
                              VKarts = _engage.GetTable<v_kart, int>(PredicateBuilder.True<v_kart>().And(x => new[] { "3494", "349402" }.Contains(x.cod_pl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_kart, int>("id",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idCard != null ? (int)y.idCard : 0))).Expand())
                                .ToList(),
                              KNaftan = _engage.GetTable<krt_Naftan, int>(PredicateExtensions.InnerContainsPredicate<krt_Naftan, long>("keykrt",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idScroll != null ? (long)y.idScroll : 0)))
                                .ToList(),
                              Etsng = item2,
                              Guild18 = new krt_Guild18 {
                                  reportPeriod = kg.Key.reportPeriod,
                                  idDeliviryNote = kg.Key.idDeliviryNote,
                                  warehouse = kg.Key.warehouse
                              }
                          }).Skip(pageSize * (page - 1)).Take(pageSize).OrderByDescending(x => x.VOtpr != null ? x.VOtpr.n_otpr : x.Guild18.idDeliviryNote.ToString()).ToList();

            return result;
        }
        
        /// <summary>
        /// Get current avaible type of operation on dispatch
        /// </summary>
        /// <param name="chooseDate"></param>
        /// <returns></returns>
        public List<short> GetTypeOfOpers(DateTime chooseDate) {
            var wrkDispatch = _engage.GetGroup<krt_Guild18, int>(x => (int)x.idDeliviryNote, x => x.reportPeriod == chooseDate && x.idDeliviryNote != null).ToList();

            var result = _engage.GetGroup<v_otpr, short>(x => (short)x.oper, x => x.state == 32 
                                && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol)) 
                                && wrkDispatch.Contains(x.id)).ToList();

            return result;
        }
        public DateTime SyncActualDate(ISessionStorage storage, DateTime menuTime) {
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

            return _engage.GetGroup<v_otpr, string>(x => x.n_otpr, x => x.n_otpr.StartsWith(templShNumber)
                 && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))
                 && x.state == 32 && (x.date_oper >= startDate && x.date_oper <= endDate))
                .OrderByDescending(x => x).Take(10);
        }
        /// <summary>
        /// Получение данных со стороны  БД САПОД (используется для предварительного просмотра текущей документации по накладным)
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <param name="preview"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public bool PackDocuments(DateTime reportPeriod, IList<ShippingInfoLineDTO> preview, byte shiftPage = 3) {
            var startDate = reportPeriod.AddDays(-shiftPage);
            var endDate = reportPeriod.AddMonths(1).AddDays(shiftPage);

            try {
                //type_doc 1 => one trunsaction (one request per one dbcontext)
                List<krt_Guild18> result = (from item in preview join vn in _engage.GetTable<v_nach, int>(PredicateBuilder.True<v_nach>().And(x => x.type_doc == 1 && new[] { "3494", "349402" }.Contains(x.cod_kl))
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
                                                idScroll = _engage.GetGroup<krt_Naftan_orc_sapod, long>(x => x.keykrt, x => x.id_kart == vn.id_kart).FirstOrDefault()
                                            }).ToList();

                foreach (var dispatch in preview) {
                    var shNumbers = dispatch.WagonsNumbers.Select(x => x.n_vag).ToList();
                    //type_doc 2 =>one trunsaction (one request per one dbcontext) (type 2 and type_doc 4 (065))
                    //in memory because not all method support entity to sql => more easy do it in memory
                    using (_engage.Uow = new UnitOfWork()) {
                        result.AddRange((from vpv in _engage.Uow.Repository<v_pam_vag>().Get_all(x => shNumbers.Contains(x.nomvag), false)
                                         join vp in _engage.Uow.Repository<v_pam>().Get_all(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl) && x.dved > startDate && x.dved < endDate, false) on vpv.id_ved equals vp.id_ved
                                         join vn in _engage.Uow.Repository<v_nach>().Get_all(x => x.type_doc == 2 && new[] { "3494", "349402" }.Contains(x.cod_kl), false)
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
                                idScroll = _engage.GetGroup<krt_Naftan_orc_sapod, long>(y => y.keykrt, z => z.id_kart == x.id_kart).FirstOrDefault()
                            }));
                    }
                    //065
                    using (_engage.Uow = new UnitOfWork()) {
                        result.AddRange((from vpv in _engage.Uow.Repository<v_pam_vag>().Get_all(x => shNumbers.Contains(x.nomvag), false) join vn in _engage.Uow.Repository<v_nach>()
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
                                    idScroll = _engage.GetGroup<krt_Naftan_orc_sapod, long>(y => y.keykrt, z => z.id_kart == x.id_kart).FirstOrDefault()
                                }));
                    }
                    ////type_doc 3 =>one trunsaction (one request per one dbcontext)
                    using (_engage.Uow = new UnitOfWork()) {
                        result.AddRange((from vav in _engage.Uow.Repository<v_akt_vag>().Get_all(x => shNumbers.Contains(x.nomvag), false)
                                         join va in _engage.Uow.Repository<v_akt>().Get_all(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl) && x.dakt > startDate && x.dakt < endDate, false) on vav.id_akt equals va.id
                                         join vn in _engage.Uow.Repository<v_nach>().Get_all(x => x.type_doc == 3 && new[] { "3494", "349402" }.Contains(x.cod_kl), false)
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
                                    idScroll = _engage.GetGroup<krt_Naftan_orc_sapod, long>(y => y.keykrt, z => z.id_kart == x.id_kart).FirstOrDefault()
                                }));

                    }
                    //luggage (type_doc 0 or 4)
                    result.AddRange(_engage.GetTable<krt_Naftan_orc_sapod, long>(x => new[] { 611, 629, 125 }.Contains(x.vidsbr) && x.dt.Month == reportPeriod.Month && x.dt.Year == reportPeriod.Year).ToList()
                        .Select(x => new krt_Guild18 {
                            reportPeriod = reportPeriod,
                            type_doc = x.tdoc,
                            idSrcDocument = (int)x.id_kart, code = x.vidsbr,
                            sum = x.sm, rateVAT = Math.Round((decimal)(x.nds / x.sm_no_nds), 2),
                            codeType = new[] { 166, 173, 300, 301, 344 }.Contains(x.vidsbr),
                            idCard = (int)x.id_kart, idScroll = x.keykrt
                        }));
                }
            } catch (Exception) {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Альтернатива PAckDocuments на стороне БД ОРЦ (т.е контрольные данные=> приходят поздже).
        /// Т.к EF6  не поддерживает работу Join с двумя котестами (таблицы находяться на разных серверах)=>
        /// Возможные варианты (выбран 3)
        /// 1)Создание view для таблиц из другого сервера 
        /// 2)ковыряние в edmx (необходимо также работа c synonem в  SQL)
        /// 3)Написание SQL прямых заросов и разметка сущностей (теряем обстракцию, т.к необходимо указывать явные имена linkid servers)
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <param name="preview"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public bool PackDocSql(DateTime reportPeriod, IList<ShippingInfoLineDTO> preview, byte shiftPage = 3) {
            var startDate = reportPeriod.AddDays(-shiftPage).Date;
            var endDate = reportPeriod.AddMonths(1).AddDays(shiftPage).Date;

            foreach (var dispatch in preview) {
                var temp = dispatch;

                using (_engage.Uow = new UnitOfWork()) {
                    //для динамического соединения
                    var sapodConn = "[" + _engage.Uow.Repository<v_otpr>().ActiveContext.Database.Connection.DataSource + @"].[" + _engage.Uow.Repository<v_otpr>().ActiveContext.Database.Connection.Database + @"]";
                    var orcConn = "[" + _engage.Uow.Repository<krt_Guild18>().ActiveContext.Database.Connection.DataSource + @"].[" + _engage.Uow.Repository<krt_Guild18>().ActiveContext.Database.Connection.Database + @"]";
                    var carriages = (temp.WagonsNumbers.Any()) ? string.Join(",", temp.WagonsNumbers.Select(x => string.Format("'{0}'", x.n_vag))) : string.Empty;

                    //Для mapping требуется точное совпадение имен и типов столбцов
                    //Выбираем с какой стороны работать (сервер) по сущности
                    var result = _engage.Uow.Repository<krt_Guild18>().ActiveContext.Database.SqlQuery<krt_Guild18>(@"
                    WITH SubResutl AS (
                        /*Doc from Invoices*/
                        SELECT @reportPeriod AS [reportPeriod],     vn.[id] AS [idSapod],     null AS [idScroll], null AS [scrollColl],
                            CASE vn.[cod_sbor] WHEN '125' THEN NULL ELSE @warehouse END AS [warehouse],   CASE vn.[cod_sbor] WHEN '125' THEN NULL ELSE @id_otpr END AS [idDeliviryNote],
                            CONVERT(tinyint,vn.type_doc) AS [type_doc],@id_otpr AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN CONVERT(int,left(vn.cod_sbor,3)) IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],
                            CONVERT(int,left(vn.[cod_sbor],3)) AS [code], vn.[summa] + vn.[nds] as [sum],
                            NULL AS [parseTextm],
                            CONVERT(decimal(18,2),vn.[nds]/vn.[summa]) as [rateVAT], vn.[id_kart] AS [idCard]
                        FROM " + sapodConn + @".[dbo].[v_nach] AS vn 
                        WHERE (vn.[id_otpr] = (@id_otpr) OR vn.[cod_sbor] IN ('125')) AND vn.[type_doc] IN (1,4) AND (vn.[date_raskr] BETWEEN @stDate AND @endDate) AND vn.[cod_kl] IN ('3494','349402')
                        
                        UNION ALL

                        /*Doc from Scrolls*/
                        SELECT DISTINCT @reportPeriod AS [reportPeriod], vn.[id] AS [idSapod], null AS [idScroll], null AS [scrollColl], @warehouse AS [warehouse],
                            @id_otpr AS [idDeliviryNote], CONVERT(tinyint,vn.[type_doc]) AS [type_doc],
                            CASE vn.[type_doc] when 2 then vp.[id_ved] ELSE vn.[id_kart] END AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN CONVERT(int,left(vn.[cod_sbor],3)) IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],
                            CONVERT(int,left(vn.[cod_sbor],3)) AS [code], vn.[summa] + vn.[nds] as [sum],
                            CASE CONVERT(int,left(vn.[cod_sbor],3)) WHEN 65 THEN ISNULL(REPLACE(SUBSTRING(REPLACE(RTRIM(LTRIM(CONVERT(VARCHAR(max),vn.[textm]))),' ',''),CHARINDEX(',',REPLACE(RTRIM(LTRIM(CAST(vn.[textm] AS VARCHAR(max)))),' ',''),6),4),',',''),0) ELSE NULL END AS [parseTextm],
                            CONVERT(decimal(18,2),vn.[nds]/vn.[summa]) as [rateVAT], vn.[id_kart] AS [idCard]
                        FROM " + sapodConn + @".[dbo].[v_pam] as vp INNER JOIN " + sapodConn + @".[dbo].[v_pam_vag] AS vpv
                            ON vpv.[id_ved] = vp.[id_ved] INNER JOIN " + sapodConn + @".[dbo].[v_nach] AS vn
                                ON ((vn.[id_kart] = vp.[id_kart] AND vn.[type_doc] = 2) AND vp.[kodkl] IN ('3494','349402')) OR
                           (vn.[date_raskr] IN (convert(date,vpv.[d_pod]),convert(date,vpv.[d_ub])) AND vn.[cod_sbor] = '065' AND vn.[type_doc] = 4)
                        WHERE vpv.[nomvag] IN (" + carriages + @") AND vn.[cod_kl] IN ('3494','349402') AND [state] = 32 AND (vp.[dved] BETWEEN @stDate AND @endDate)
                        
                        UNION ALL

                        /*Doc from Act*/
                        SELECT @reportPeriod AS [reportPeriod], vn.id AS [idSapod], null AS [idScroll], null AS [scrollColl], @warehouse AS [warehouse],
                            @id_otpr AS [idDeliviryNote], CONVERT(tinyint,vn.type_doc) AS [type_doc], va.id AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN CONVERT(int,left(vn.cod_sbor,3)) IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],
                            CONVERT(int,left(vn.cod_sbor,3)) AS [code], vn.summa + vn.nds as [sum],
                            NULL AS [parseTextm],
                            CONVERT(decimal(18,2),vn.nds/vn.summa) as [rateVAT], vn.id_kart AS [idCard]
                        FROM " + sapodConn + @".[dbo].v_akt as va INNER JOIN " + sapodConn + @".[dbo].[v_nach] AS vn
                                ON vn.id_kart = va.id_kart AND vn.type_doc = 3 AND va.kodkl IN ('3494','349402') AND vn.cod_kl IN ('3494','349402') AND [state] = 32
                        WHERE Exists (SELECT * from " + sapodConn + @".[dbo].v_akt_vag as vav where vav.nomvag IN (" + carriages + @") and va.id = vav.id_akt) AND (va.dakt BETWEEN @stDate AND @endDate)
                    )
                        
                        SELECT 0 as [id], sr.reportPeriod,sr.idSapod, knos.keykrt AS [idScroll],knos.keysbor AS [scrollColl], sr.warehouse, sr.idDeliviryNote,sr.type_doc, sr.idSrcDocument,
                        CONVERT(BIT,CASE WHEN CONVERT(int,sr.codeType) IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],
                        sr.code, CASE sr.code 
							WHEN 65 THEN  
								ROUND(ISNULL(ISNULL(knos.sm,sr.[sum]) / cast('' as xml).value('sql:column(" + @"""" + "parseTextm" + @"""" + @") cast as xs:integer ?', 'int') * @countCarriages,0),4)
                            ELSE ISNULL(knos.sm, sr.[sum])
                        END AS[sum], sr.[rateVat],sr.idCard
                        FROM SubResutl AS sr LEFT JOIN " + orcConn + @".[dbo].[krt_Naftan_orc_sapod] AS knos
	                        ON knos.id = sr.idSapod AND knos.tdoc = sr.type_doc
                        
                        UNION ALL 
                        
                        SELECT 0 as [id], @reportPeriod AS [reportPeriod], knos.id AS [idSapod],
                            kn.keykrt AS [idScroll],knos.keysbor AS [scrollColl],@warehouse AS [warehouse], NULL AS [idDeliviryNote], tdoc as [type_doc],
                            CASE tdoc when 4 THEN knos.id_kart else NULL END AS [idSrcDocument],
                            CONVERT(BIT,1) AS [codeType], vidsbr AS [code], sm as [sum],
                            CONVERT(decimal(18,2),knos.stnds/100) as [rateVAT],knos.id_kart AS [idCard]
                        FROM " + orcConn + @".[dbo].krt_naftan AS kn INNER JOIN " + orcConn + @".[dbo].krt_Naftan_orc_sapod AS knos
	                        ON knos.keykrt = kn.keykrt AND KN.U_KOD = 2
                        WHERE knos.dt BETWEEN @stDate AND @endDate;",
                        new SqlParameter("@reportPeriod", reportPeriod),
                        new SqlParameter("@warehouse", temp.Warehouse),
                        new SqlParameter("@id_otpr", (temp.Shipping == null) ? 0 : temp.Shipping.id),
                        new SqlParameter("@stDate", startDate),
                        new SqlParameter("@endDate", endDate),
                        new SqlParameter("@countCarriages", temp.WagonsNumbers.Count)).ToList();

                    //Add or Delete
                    foreach (var entity in result) {
                        var e = entity;
                        var item = _engage.Uow.Repository<krt_Guild18>().Get(x => x.reportPeriod == reportPeriod && x.idSapod == e.idSapod && x.scrollColl == e.scrollColl && x.idScroll == e.idScroll && x.idDeliviryNote == e.idDeliviryNote);

                        entity.id = (item == null) ? 0 : item.id;
                        _engage.Uow.Repository<krt_Guild18>().Merge(entity);
                    }
                    _engage.Uow.Save();
                }
            }

            return true;
        }
        public bool DeleteInvoice(DateTime reportPeriod, int? idInvoice) {
            using (_engage.Uow = new UnitOfWork()) {
                try {
                    _engage.Uow.Repository<krt_Guild18>().Delete(x => x.reportPeriod == reportPeriod && x.idDeliviryNote == idInvoice, false);
                } catch (Exception) {
                    return false;
                }
                //Some way to see generate SQL code
                //var sql = ((System.Data.Entity.Core.Objects.ObjectQuery)query).ToTraceString();
                _engage.Uow.Save();
            }
            return true;
        }
        /// <summary>
        /// Update existing information in definite period time
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <returns></returns>
        public bool UpdateExists(DateTime reportPeriod) {
            var dataRows = _engage.GetTable<krt_Guild18, int?>(x => x.reportPeriod == reportPeriod && x.idDeliviryNote != null).GroupBy(x => new { x.idDeliviryNote, x.warehouse })
                .Select(y => new ShippingInfoLineDTO() {
                    Shipping = _engage.GetTable<v_otpr, int>(x => x.id == y.Key.idDeliviryNote).First(),
                    WagonsNumbers = _engage.GetTable<v_o_v, int>(x => x.id_otpr == y.Key.idDeliviryNote).ToList(),
                    Warehouse = (int)y.Key.warehouse
                }).ToList();

            PackDocSql(reportPeriod, dataRows);

            return true;
        }
        public IEnumerable<ShippingInfoLineDTO> ShippingPreview(string deliveryNote, DateTime dateOper, out short recordCount) {
            DateTime startDate = dateOper.AddDays(-5);
            DateTime endDate = dateOper.AddMonths(1).AddDays(5);
            IEnumerable<ShippingInfoLineDTO> result = new List<ShippingInfoLineDTO>();
             
            var delivery = _engage.GetTable<v_otpr, int>(x => x.n_otpr == deliveryNote && x.state == 32 && x.date_oper >= startDate && x.date_oper <= endDate &&
               (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))).ToList();
            
            if (!delivery.Any()) {
                recordCount = 0;
            } else {
                //one trunsaction (one request per one dbcontext)
                using (_engage.Uow = new UnitOfWork()) {
                    result = (from sh in delivery join e in _engage.Uow.Repository<etsng>().Get_all(enableDetectChanges: false) on sh.cod_tvk_etsng equals e.etsng1
                              select new ShippingInfoLineDTO() {
                                  Shipping = sh,
                                  CargoEtsngName = e,
                                  WagonsNumbers = _engage.GetTable<v_o_v, int>(x => x.id_otpr == sh.id).ToList(),
                              }).ToList();
                }
                recordCount = (short)result.Count();
            }
            return result;
        }

        protected override void DisposeCore() {
            if (_engage != null)
                _engage.Dispose();
        }
    }
}