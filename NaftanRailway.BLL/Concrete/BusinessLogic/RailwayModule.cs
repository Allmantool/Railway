using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using LinqKit;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Guild18;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.Services.ExpressionTreeExtensions;
using Railway.Core;
using Railway.Core.Data.EF;
using Railway.Domain.Concrete.DbContexts.Mesplan.Entities;
using Railway.Domain.Concrete.DbContexts.OBD.Entities;
using Railway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.BLL.Concrete.BusinessLogic
{
    public class RailwayModule : Disposable, IRailwayModule
    {
        private readonly IBusinessProvider _provider;

        public RailwayModule(IBusinessProvider provider)
        {
            this._provider = provider;
        }

        /// <summary>
        /// Формирования объекта отображения информации об отправках (по накладной за отчётный месяц)
        /// </summary>
        public IEnumerable<ShippingDTO> ShippingsViews(EnumOperationType operationCategory, DateTime chooseDate, int page, int pageSize, out short recordCount)
        {

            if (this._provider.GetCountRows<krt_Guild18>(x => x.reportPeriod == chooseDate) == 0)
            {
                recordCount = 0;
                return new List<ShippingDTO>();
            }
            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together, resolve through expression tree maybe)
            var wrkData = this._provider.GetTable<krt_Guild18, int>(x => x.reportPeriod == chooseDate).ToList();

            //dispatch
            var kg18Src = wrkData.GroupBy(x => new { x.reportPeriod, x.idDeliviryNote, x.warehouse })
                .OrderBy(x => x.Key.idDeliviryNote).ToList();

            /*linqkit*/
            //v_otpr
            var votprPredicate = PredicateBuilder.New<v_otpr>(false)
                .DefaultExpression
                .And(x => ((x.oper == (short)operationCategory) || operationCategory == EnumOperationType.All)
                          && x.state == 32 
                          && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) 
                              || new[] { "3494", "349402" }.Contains(x.cod_klient_pol)));
            votprPredicate = kg18Src.Select(x => x.Key.idDeliviryNote).Aggregate(votprPredicate, (current, value) => current.Or(e => e.id == value && ((e.oper == (short)operationCategory) || operationCategory == EnumOperationType.All))).Expand();

            var voSrc = this._provider.GetTable<v_otpr, int>(votprPredicate).ToList();
            recordCount = (short)voSrc.Count();
            //v_o_v
            //var vovPredicate = PredicateBuilder.New<v_o_v>(false).DefaultExpression;
            var vovPredicate = voSrc.Select(x => x.id).Aggregate(PredicateBuilder.New<v_o_v>(false).DefaultExpression, (current, value) => current.Or(v => v.id_otpr == value)).Expand();
            var vovSrc = this._provider.GetTable<v_o_v, int>(vovPredicate).ToList();
            //etsng
            //var etsngPredicate = PredicateBuilder.New<etsng>(false);
            var etsngPredicate = voSrc.Select(x => x.cod_tvk_etsng).Aggregate(PredicateBuilder.New<Etsng>(false).DefaultExpression, (current, value) => current.Or(v => v.etsng1 == value)).Expand();
            var etsngSrc = this._provider.GetTable<Etsng, int>(etsngPredicate).ToList();


            var result = (from kg in kg18Src
                          join vo in voSrc on kg.Key.idDeliviryNote equals vo.id into g1
                          from item in g1.DefaultIfEmpty()
                          where (item != null && item.oper == (short)operationCategory) || operationCategory == EnumOperationType.All
                          join e in etsngSrc on item == null ? "" : item.cod_tvk_etsng equals e.Etsng1 into g2
                          from item2 in g2.DefaultIfEmpty()
                          select new ShippingDTO()
                          {
                              VOtpr = item,
                              Vovs = vovSrc.Where(x => (x != null) && x.id_otpr == (item == null ? 0 : item.id)),

                              VPams = this._provider.GetTable<v_pam, int>(PredicateBuilder.New<v_pam>().DefaultExpression.And(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_pam, int>("id_ved",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 2).Select(y => y.idSrcDocument != null ? (int)y.idSrcDocument : 0))).Expand())
                                .ToList(),
                              VAkts = this._provider.GetTable<v_akt, int>(PredicateBuilder.New<v_akt>().DefaultExpression.And(x => new[] { "3494", "349402" }.Contains(x.kodkl) && x.state == 32)
                                .And(PredicateExtensions.InnerContainsPredicate<v_akt, int>("id",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 3).Select(y => y.idSrcDocument != null ? (int)y.idSrcDocument : 0))).Expand())
                                .ToList(),
                              VKarts = this._provider.GetTable<v_kart, int>(PredicateBuilder.New<v_kart>().DefaultExpression.And(x => new[] { "3494", "349402" }.Contains(x.cod_pl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_kart, int>("id",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idCard != null ? (int)y.idCard : 0))).Expand())
                                .ToList(),
                              KNaftan = this._provider.GetTable<krt_Naftan, int>(PredicateExtensions.InnerContainsPredicate<krt_Naftan, long>("keykrt",

                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idScroll != null ? (long)y.idScroll : 0)))
                                .ToList(),
                              Etsng = item2,
                              Guild18 = new krt_Guild18
                              {
                                  reportPeriod = kg.Key.reportPeriod,
                                  idDeliviryNote = kg.Key.idDeliviryNote,
                                  warehouse = kg.Key.warehouse
                              }
                          }).Skip(pageSize * (page - 1)).Take(pageSize).OrderByDescending(x => x.VOtpr != null ? x.VOtpr.n_otpr : x.Guild18.idDeliviryNote.ToString()).ToList();

            return result;
        }

        /// <summary>
        /// Get current available type of operation on dispatch
        /// </summary>
        /// <param name="chooseDate"></param>
        /// <returns></returns>
        public List<short> GetTypeOfOpers(DateTime chooseDate)
        {
            var wrkDispatch = this._provider.GetGroup<krt_Guild18, int>(x => (int)x.idDeliviryNote, x => x.reportPeriod == chooseDate && x.idDeliviryNote != null).Select(y => y.First().idDeliviryNote);

            var result = this._provider.GetGroup<v_otpr, short>(x => (short)x.oper, x => x.state == 32
                                                                                         && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))
                                && wrkDispatch.Contains(x.id) && x.oper != null).Select(x => x.First().oper.Value).ToList();

            return result;
        }

        public DateTime SyncActualDate(ISessionStorage storage, DateTime menuTime)
        {
            //reload page (save select report date)
            if (storage.ReportPeriod == DateTime.MinValue && menuTime == DateTime.MinValue)
            {
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }

            if (storage.ReportPeriod != DateTime.MinValue && menuTime == DateTime.MinValue)
            {
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
        public IEnumerable<string> AutoCompleteShipping(string templShNumber, DateTime chooseDate, byte shiftPage = 3)
        {
            var startDate = chooseDate.AddDays(-shiftPage);
            var endDate = chooseDate.AddMonths(1).AddDays(shiftPage);

            IEnumerable<string> result;

            try
            {
                result = this._provider.GetGroup<v_nach, string>(x => x.num_doc,
                        x => x.num_doc.StartsWith(templShNumber)
                                        && (new[] { "3494", "349402" }.Contains(x.cod_kl))
                                        && x.type_doc == 1 && (x.date_raskr >= startDate && x.date_raskr <= endDate))
                                .Select(x => x.First().num_doc).OrderByDescending(x => x).Take(10);
            }
            catch (Exception ex)
            {
                this._provider.Log.DebugFormat($"AutoComplete method throws exception: {ex.Message}.");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Получение данных со стороны  БД САПОД (используется для предварительного просмотра текущей документации по накладным)
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <param name="preview"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        public bool PackDocuments(DateTime reportPeriod, IList<ShippingInfoLineDTO> preview, byte shiftPage = 3)
        {
            var startDate = reportPeriod.AddDays(-shiftPage);
            var endDate = reportPeriod.AddMonths(1).AddDays(shiftPage);

            try
            {
                //type_doc 1 => one transaction (one request per one dbcontext)
                List<krt_Guild18> result = (from item in preview
                                            join vn in this._provider.GetTable<v_nach, int>(PredicateBuilder.New<v_nach>().DefaultExpression
       .And(x => x.type_doc == 1 && new[] { "3494", "349402" }.Contains(x.cod_kl))
                 .And(PredicateExtensions.InnerContainsPredicate<v_nach, int?>("id_otpr", preview.Select(x => (int?)x.Shipping.id))).Expand())
         on item.Shipping.id equals vn.id_otpr
                                            select new krt_Guild18()
                                            {
                                                reportPeriod = reportPeriod,
                                                warehouse = item.Warehouse,
                                                idDeliviryNote = item.Shipping.id,
                                                type_doc = 1,
                                                idSrcDocument = item.Shipping.id,
                                                code = Convert.ToInt32(vn.cod_sbor.Split(new[] { '.', ',' })[0]),
                                                sum = (decimal)(vn.summa + vn.nds),
                                                rateVAT = Math.Round((decimal)(vn.nds / vn.summa), 2),
                                                codeType = new[] { 166, 173, 300, 301, 344 }.Contains(Convert.ToInt32(vn.cod_sbor.Split(new[] { '.', ',' })[0])),
                                                idCard = vn.id_kart,
                                                idScroll = this._provider.GetGroup<krt_Naftan_orc_sapod, long>(x => x.keykrt, x => x.id_kart == vn.id_kart).Select(y => y.First().keykrt).FirstOrDefault()
                                            }).ToList();

                foreach (var dispatch in preview)
                {
                    var shNumbers = dispatch.WagonsNumbers.Select(x => x.n_vag).ToList();
                    //type_doc 2 =>one transaction (one request per one dbcontext) (type 2 and type_doc 4 (065))
                    //in memory because not all method support entity to sql => more easy do it in memory
                    using (this._provider.Uow = new UnitOfWork())
                    {
                        result.AddRange((from vpv in this._provider.Uow.GetRepository<v_pam_vag>().GetAll(x => shNumbers.Contains(x.nomvag), false)
                                         join vp in this._provider.Uow.GetRepository<v_pam>().GetAll(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl) && x.dved > startDate && x.dved < endDate, false) on vpv.id_ved equals vp.id_ved
                                         join vn in this._provider.Uow.GetRepository<v_nach>().GetAll(x => x.type_doc == 2 && new[] { "3494", "349402" }.Contains(x.cod_kl), false)
                                             on vp.id_kart equals vn.id_kart
                                         select new { vp.id_ved, vn.cod_sbor, vn.summa, vn.nds, vn.id_kart }).Distinct().ToList()
                            .Select(x => new krt_Guild18
                            {
                                reportPeriod = reportPeriod,
                                warehouse = dispatch.Warehouse,
                                idDeliviryNote = dispatch.Shipping.id,
                                type_doc = 2,
                                idSrcDocument = x.id_ved,
                                code = Convert.ToInt32(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                sum = (decimal)(x.summa + x.nds),
                                idCard = x.id_kart,
                                rateVAT = Math.Round((decimal)(x.nds / x.summa), 2),
                                codeType = new[] { "166", "173", "300", "301", "344" }.Contains(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                idScroll = this._provider.GetGroup<krt_Naftan_orc_sapod, long>(y => y.keykrt, z => z.id_kart == x.id_kart).Select(y => y.First().keykrt).FirstOrDefault()
                            }));
                    }
                    //065
                    using (this._provider.Uow = new UnitOfWork())
                    {
                        result.AddRange((from vpv in this._provider.Uow.GetRepository<v_pam_vag>().GetAll(x => shNumbers.Contains(x.nomvag), false)
                                         join vn in this._provider.Uow.GetRepository<v_nach>()
.GetAll(x => x.type_doc == 4 && x.cod_sbor == "065" && new[] { "3494", "349402" }.Contains(x.cod_kl) && x.date_raskr > startDate && x.date_raskr < endDate, false) on
new { p1 = vpv.d_pod, p2 = vpv.d_ub } equals new { p1 = vn.date_raskr, p2 = vn.date_raskr }
                                         select new { vpv.id_ved, vn.cod_sbor, vn.summa, vn.nds, vn.id_kart }).Distinct().ToList()
                            .Select(x =>
                                new krt_Guild18
                                {
                                    reportPeriod = reportPeriod,
                                    warehouse = dispatch.Warehouse,
                                    idDeliviryNote = dispatch.Shipping.id,
                                    type_doc = 4,
                                    idSrcDocument = x.id_kart,
                                    code = Convert.ToInt32(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    sum = (decimal)(x.summa + x.nds),
                                    idCard = x.id_kart,
                                    rateVAT = Math.Round((decimal)(x.nds / x.summa), 2),
                                    codeType = new[] { "166", "173", "300", "301", "344" }.Contains(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    idScroll = this._provider.GetGroup<krt_Naftan_orc_sapod, long>(y => y.keykrt, z => z.id_kart == x.id_kart).Select(y => y.First().keykrt).FirstOrDefault()
                                }));
                    }
                    ////type_doc 3 =>one transaction (one request per one dbcontext)
                    using (this._provider.Uow = new UnitOfWork())
                    {
                        result.AddRange((from vav in this._provider.Uow.GetRepository<v_akt_vag>().GetAll(x => shNumbers.Contains(x.nomvag), false)
                                         join va in this._provider.Uow.GetRepository<v_akt>().GetAll(x => x.state == 32 && new[] { "3494", "349402" }.Contains(x.kodkl) && x.dakt > startDate && x.dakt < endDate, false) on vav.id_akt equals va.id
                                         join vn in this._provider.Uow.GetRepository<v_nach>().GetAll(x => x.type_doc == 3 && new[] { "3494", "349402" }.Contains(x.cod_kl), false)
                                             on va.id_kart equals vn.id_kart
                                         select new { va.id, vn.cod_sbor, vn.summa, vn.nds, vn.id_kart }).Distinct().ToList()
                            .Select(x =>
                                new krt_Guild18
                                {
                                    reportPeriod = reportPeriod,
                                    warehouse = dispatch.Warehouse,
                                    idDeliviryNote = dispatch.Shipping.id,
                                    type_doc = 3,
                                    idSrcDocument = x.id,
                                    code = Convert.ToInt32(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    sum = (decimal)(x.summa + x.nds),
                                    idCard = x.id_kart,
                                    rateVAT = Math.Round((decimal)(x.nds / x.summa), 2),
                                    codeType = new[] { "166", "173", "300", "301", "344" }.Contains(x.cod_sbor.Split(new[] { '.', ',' })[0]),
                                    idScroll = this._provider.GetGroup<krt_Naftan_orc_sapod, long>(y => y.keykrt, z => z.id_kart == x.id_kart).Select(y => y.First().keykrt).FirstOrDefault()
                                }));

                    }
                    //luggage (type_doc 0 or 4)
                    result.AddRange(this._provider.GetTable<krt_Naftan_orc_sapod, long>(x => new[] { 611, 629, 125 }.Contains(x.vidsbr) && x.dt.Month == reportPeriod.Month && x.dt.Year == reportPeriod.Year).ToList()
                        .Select(x => new krt_Guild18
                        {
                            reportPeriod = reportPeriod,
                            type_doc = x.tdoc,
                            idSrcDocument = (int)x.id_kart,
                            code = x.vidsbr,
                            sum = x.sm,
                            rateVAT = Math.Round((decimal)(x.nds / x.sm_no_nds), 2),
                            codeType = new[] { 166, 173, 300, 301, 344 }.Contains(x.vidsbr),
                            idCard = (int)x.id_kart,
                            idScroll = x.keykrt
                        }));
                }
            }
            catch (Exception)
            {
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
        public bool PackDocSql(DateTime reportPeriod, IList<ShippingInfoLineDTO> preview, byte shiftPage = 3)
        {
            var startDate = reportPeriod.AddDays(-shiftPage).Date;
            var endDate = reportPeriod.AddMonths(1).AddDays(shiftPage).Date;

            foreach (var dispatch in preview)
            {
                var temp = dispatch;

                using (this._provider.Uow = new UnitOfWork())
                {
                    var sapodConn = this._provider.Uow.GetRepository<v_otpr>().GetCurrentConnection();
                    var orcConn = "[" + this._provider.Uow.GetRepository<krt_Guild18>().GetCurrentConnection();

                    var carriages = (temp.WagonsNumbers.Any()) ? string.Join(",", temp.WagonsNumbers.Select(x => $"'{x.n_vag}'")) : string.Empty;

                    var sqlParameters = new[]
                    {
                        new SqlParameter("@reportPeriod", reportPeriod),
                        new SqlParameter("@warehouse", temp.Warehouse),
                        new SqlParameter("@id_otpr", (temp.Shipping == null) ? 0 : temp.Shipping.id),
                        new SqlParameter("@stDate", startDate),
                        new SqlParameter("@endDate", endDate),
                        new SqlParameter("@countCarriages", temp.WagonsNumbers.Count)
                    };

                    var result = this._provider.Uow.GetRepository<krt_Guild18>().SqlQuery<krt_Guild18>(@"
                    WITH SubResutl AS (
                        /*Doc from Invoices*/
                        SELECT @reportPeriod AS [reportPeriod],     vn.[Id] AS [idSapod],     null AS [idScroll], null AS [scrollColl],
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
                        SELECT DISTINCT @reportPeriod AS [reportPeriod], vn.[Id] AS [idSapod], null AS [idScroll], null AS [scrollColl], @warehouse AS [warehouse],
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
                        SELECT @reportPeriod AS [reportPeriod], vn.Id AS [idSapod], null AS [idScroll], null AS [scrollColl], @warehouse AS [warehouse],
                            @id_otpr AS [idDeliviryNote], CONVERT(tinyint,vn.type_doc) AS [type_doc], va.Id AS [idSrcDocument],
                            CONVERT(BIT,CASE WHEN CONVERT(int,left(vn.cod_sbor,3)) IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],
                            CONVERT(int,left(vn.cod_sbor,3)) AS [code], vn.summa + vn.nds as [sum],
                            NULL AS [parseTextm],
                            CONVERT(decimal(18,2),vn.nds/vn.summa) as [rateVAT], vn.id_kart AS [idCard]
                        FROM " + sapodConn + @".[dbo].v_akt as va INNER JOIN " + sapodConn + @".[dbo].[v_nach] AS vn
                                ON vn.id_kart = va.id_kart AND vn.type_doc = 3 AND va.kodkl IN ('3494','349402') AND vn.cod_kl IN ('3494','349402') AND [state] = 32
                        WHERE Exists (SELECT * from " + sapodConn + @".[dbo].v_akt_vag as vav where vav.nomvag IN (" + carriages + @") and va.Id = vav.id_akt) AND (va.dakt BETWEEN @stDate AND @endDate)
                    )

                        SELECT 0 as [Id], sr.reportPeriod,sr.idSapod, knos.keykrt AS [idScroll],knos.keysbor AS [scrollColl], sr.warehouse, sr.idDeliviryNote,sr.type_doc, sr.idSrcDocument,
                        CONVERT(BIT,CASE WHEN CONVERT(int,sr.codeType) IN (166,173,300,301,344) THEN 0 ELSE 1 END) AS [codeType],
                        sr.code, CASE sr.code
							WHEN 65 THEN
								ROUND(ISNULL(ISNULL(knos.sm,sr.[sum]) / cast('' as xml).value('sql:column(" + @"""" + "parseTextm" + @"""" + @") cast as xs:integer ?', 'int') * @countCarriages,0),4)
                            ELSE ISNULL(knos.sm, sr.[sum])
                        END AS[sum], sr.[rateVat],sr.idCard
                        FROM SubResutl AS sr LEFT JOIN " + orcConn + @".[dbo].[krt_Naftan_orc_sapod] AS knos
	                        ON knos.Id = sr.idSapod AND knos.tdoc = sr.type_doc

                        UNION ALL

                        SELECT 0 as [Id], @reportPeriod AS [reportPeriod], knos.Id AS [idSapod],
                            kn.keykrt AS [idScroll],knos.keysbor AS [scrollColl],@warehouse AS [warehouse], NULL AS [idDeliviryNote], tdoc as [type_doc],
                            CASE tdoc when 4 THEN knos.id_kart else NULL END AS [idSrcDocument],
                            CONVERT(BIT,1) AS [codeType], vidsbr AS [code], sm as [sum],
                            CONVERT(decimal(18,2),knos.stnds/100) as [rateVAT],knos.id_kart AS [idCard]
                        FROM " + orcConn + @".[dbo].krt_naftan AS kn INNER JOIN " + orcConn + @".[dbo].krt_Naftan_orc_sapod AS knos
	                        ON knos.keykrt = kn.keykrt AND KN.U_KOD = 2
                        WHERE knos.dt BETWEEN @stDate AND @endDate;",
                        sqlParameters).ToList();

                    try
                    {
                        //Add or Delete
                        foreach (var entity in result)
                        {
                            var e = entity;
                            var item = this._provider.Uow.GetRepository<krt_Guild18>().Get(x => x.reportPeriod == reportPeriod && x.idSapod == e.idSapod && x.scrollColl == e.scrollColl && x.idScroll == e.idScroll && x.idDeliviryNote == e.idDeliviryNote);

                            entity.id = (item == null) ? 0 : item.id;
                            this._provider.Uow.GetRepository<krt_Guild18>().Merge(entity);
                        }

                        this._provider.Uow.Save();

                    }
                    catch (Exception ex)
                    {
                        this._provider.Log.DebugFormat("Exception: {0}", ex.Message);

                        return false;
                    }
                }
            }

            return true;
        }

        public bool DeleteInvoice(DateTime reportPeriod, int? idInvoice)
        {
            using (this._provider.Uow = new UnitOfWork())
            {
                try
                {
                    this._provider.Uow.GetRepository<krt_Guild18>().Delete(x => x.reportPeriod == reportPeriod && x.idDeliviryNote == idInvoice, false);
                }
                catch (Exception)
                {
                    return false;
                }
                //Some way to see generate SQL code
                //var sql = ((System.Data.Entity.Core.Objects.ObjectQuery)query).ToTraceString();
                this._provider.Uow.Save();
            }
            return true;
        }

        /// <summary>
        /// Update existing information in definite period time
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <returns></returns>
        public bool UpdateExists(DateTime reportPeriod)
        {
            var dataRows = this._provider.GetTable<krt_Guild18, int?>(x => x.reportPeriod == reportPeriod && x.idDeliviryNote != null).GroupBy(x => new { x.idDeliviryNote, x.warehouse })
                .Select(y => new ShippingInfoLineDTO()
                {
                    Shipping = this._provider.GetTable<v_otpr, int>(x => x.id == y.Key.idDeliviryNote).First(),
                    WagonsNumbers = this._provider.GetTable<v_o_v, int>(x => x.id_otpr == y.Key.idDeliviryNote).ToList(),
                    Warehouse = (int)y.Key.warehouse
                }).ToList();

            this.PackDocSql(reportPeriod, dataRows);

            return true;
        }

        public IEnumerable<ShippingInfoLineDTO> ShippingPreview(string deliveryNote, DateTime dateOper, out short recordCount)
        {
            DateTime startDate = dateOper.AddDays(-5);
            DateTime endDate = dateOper.AddMonths(1).AddDays(5);
            IEnumerable<ShippingInfoLineDTO> result = new List<ShippingInfoLineDTO>();

            var delivery = this._provider.GetTable<v_otpr, int>(x => x.n_otpr == deliveryNote && x.state == 32 && x.date_oper >= startDate && x.date_oper <= endDate &&
                                                                     (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))).ToList();

            if (!delivery.Any())
            {
                recordCount = 0;
            }
            else
            {
                //one transaction (one request per one dbcontext)

                using (this._provider.Uow = new UnitOfWork())
                {
                    result = (from sh in delivery
                              join e in this._provider.Uow.GetRepository<Etsng>().GetAll(enableDetectChanges: false) on sh.cod_tvk_etsng equals e.etsng1
                              select new ShippingInfoLineDTO()
                              {
                                  Shipping = sh,
                                  CargoEtsngName = e,
                                  WagonsNumbers = this._provider.GetTable<v_o_v, int>(x => x.id_otpr == sh.id).ToList(),
                              }).ToList();
                }
                recordCount = (short)result.Count();
            }
            return result;
        }

        public IEnumerable<OverviewCarriageDTO> EstimatedCarrieages()
        {
            DateTime supremePeriod = DateTime.Today.AddDays(-10);
            DateTime currentMonth = DateTime.Today.AddDays(-15);

            //code of goods that not necessary to funded
            var outSearch = new[] { "" };

            var estimatedCarriages = this._provider.GetTable<v_OPER_ASUS, int>(x =>
                    x.time_oper >= supremePeriod &&
                    x.cod_oper == "01" &&
                    x.cod_grpl == "3494" &&
                    x.ves_gruz > 0 &&
                    !outSearch.Contains(x.cod_gruz) &&
                    !x.cod_gruz.StartsWith("421")
               ).OrderByDescending(x => x.time_oper).ToList();

            var estimatedAltCarriages = this._provider.GetTable<v_02_podhod, DateTime?>(x =>
                x.date_oper_v >= currentMonth &&
                    x.kod_pol == "3494" &&
                    !outSearch.Contains(x.kod_etsng) &&
                    !x.kod_etsng.StartsWith("421")
                ).OrderByDescending(x => x.date_oper_v).ToList();

            //return cargo name
            Func<string, string> cargoName = (cod) =>
            {
                var request = this._provider.GetTable<Etsng, int>(x => x.etsng1.Substring(0, 5) == cod).FirstOrDefault();

                return request == null ? String.Empty : request.Name;
            };

            //merge two object of the same type to one (we have 2 different source)
            var result = estimatedCarriages.Select(cr => new OverviewCarriageDTO()
            {
                Carriage = cr,
                Cargo = cargoName(cr.cod_gruz),
            }).Concat(estimatedAltCarriages.Select(cr => new OverviewCarriageDTO()
            {
                AltCarriage = cr,
                AltCargo = cargoName(cr.kod_etsng)
            }));

            return result;
        }

        protected override void ExtenstionDispose()
        {
            this._provider?.Dispose();
        }
    }
}