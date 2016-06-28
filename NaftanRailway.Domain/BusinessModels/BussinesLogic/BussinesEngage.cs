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
                              /*Uow.ActiveContext.Database.SqlQuery<v_pam>(@"
                                SELECT vp.id_ved,vp.nved,vp.dzakr,vp.id_kart,vp.nkrt 
                                FROM [obd].[dbo].v_pam as vp
                                WHERE vp.[kodkl] IN ('3494','349402') AND vp.[state] = 32 AND vp.id_ved in 
                                    (SELECT src.idSrcDocument 
                                    FROM [db2].[nsd2].[dbo].[krt_Guild18] as src 
                                    WHERE type_doc = 2 AND vp.id_ved = src.idSrcDocument AND src.reportPeriod = @param1 AND src.idDeliviryNote = @param2);",
                                 new SqlParameter("@param1", chooseDate),
                                 new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),*/
                              VAkts = GetTable<v_akt, int>(PredicateBuilder.True<v_akt>().And(x => new[] { "3494", "349402" }.Contains(x.kodkl) && x.state == 32)
                                .And(PredicateExtensions.InnerContainsPredicate<v_akt, int>("id",
                                    wrkData.Where(x => x.reportPeriod == chooseDate && x.idDeliviryNote == (item != null ? item.id : 0) && x.type_doc == 3).Select(y => (int)y.idSrcDocument))).Expand())
                                .ToList(),
                              /*Uow.ActiveContext.Database.SqlQuery<v_akt>(@"
                                SELECT nakt,dakt,nkrt,id_kart
                                FROM [obd].[dbo].v_akt as va 
                                WHERE kodkl IN ('3494','349402') AND VA.[state] =32 AND va.id IN 
                                    (SELECT src.idSrcDocument FROM [db2].[nsd2].[dbo].[krt_Guild18] as src 
                                    WHERE src.type_doc = 3 AND va.id = src.idSrcDocument 
                                        AND src.reportPeriod = @param1 AND src.idDeliviryNote = @param2);",
                              new SqlParameter("@param1", chooseDate),
                              new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),*/
                              VKarts = GetTable<v_kart, int>(PredicateBuilder.True<v_kart>().And(x => new[] { "3494", "349402" }.Contains(x.cod_pl))
                                .And(PredicateExtensions.InnerContainsPredicate<v_kart, int>("id",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idCard))).Expand())
                                .ToList(),
                              /*Uow.ActiveContext.Database.SqlQuery<v_kart>(@"
                                SELECT vk.id,vk.num_kart,vk.date_okrt,vk.summa,vk.date_fdu93,vk.date_zkrt
                                FROM [obd].[dbo].v_kart as vk   
                                WHERE vk.cod_pl in ('3494','349402') AND vk.id IN 
                                    (SELECT idCard FROM [db2].[nsd2].[dbo].[krt_Guild18] as src WHERE idCard = vk.id  
                                    AND src.reportPeriod = @param1 AND src.idDeliviryNote = @param2 OR (ISNULL(@param2,0) = 0 AND src.idDeliviryNote IS NULL));",
                              new SqlParameter("@param1", chooseDate),
                              new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),*/
                              KNaftan = GetTable<krt_Naftan, int>(PredicateExtensions.InnerContainsPredicate<krt_Naftan, long>("keykrt",
                                    wrkData.Where(z => z.reportPeriod == chooseDate && z.idDeliviryNote == (item != null ? item.id : (int?)null)).Select(y => y.idScroll)))
                                .ToList(),
                              /* Uow.ActiveContext.Database.SqlQuery<krt_Naftan>(@"
                                 WITH src AS (SELECT reportPeriod,idDeliviryNote,idScroll FROM [db2].[nsd2].[dbo].krt_Guild18 GROUP BY reportPeriod,idDeliviryNote,idScroll)
                                 SELECT src.idDeliviryNote, KEYKRT,nkrt,NTREB,DTBUHOTCHET,EndDate_Per,DTOPEN,SMTREB,NDSTREB,P_TYPE,RecordCount,Scroll_Sbor
                                 FROM src inner join [db2].[nsd2].[dbo].[krt_Naftan] AS kn
                                     ON src.idScroll = kn.KEYKRT
                                 WHERE src.reportPeriod = @param1 AND src.idDeliviryNote = @param2 OR (ISNULL(@param2,0) = 0 AND src.idDeliviryNote IS NULL);",
                                 new SqlParameter("@param1", chooseDate),
                                 new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),*/
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
        public IEnumerable<string> AutoCompleteShipping(string templShNumber, DateTime chooseDate, byte shiftPage = 7) {
            var startDate = chooseDate.AddDays(-shiftPage);
            var endDate = chooseDate.AddDays(shiftPage);

            return GetGroup<v_otpr, string>(x => new { x.n_otpr }.n_otpr, x => x.n_otpr.StartsWith(templShNumber)
                 && (new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) || new[] { "3494", "349402" }.Contains(x.cod_klient_pol))
                 && x.state == 32 && (x.date_oper >= startDate && x.date_oper <= endDate))
                .OrderByDescending(x => x).Take(10);
        }
        public IEnumerable<ShippingInfoLine> PackDocuments(string deliveryNote, DateTime dateOper, out short recordCount) {
            DateTime startDate = dateOper.AddDays(-7);
            DateTime endDate = dateOper.AddMonths(1).AddDays(7);
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
                                  WagonsNumbers = GetTable<v_o_v, int>(x => x.id_otpr == sh.id),
                              }).ToList();
                }
                recordCount = (short)result.Count();
            }
            return result;
        }
        public IQueryable<Shipping> ShippingInformation {
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
        /// Count operation throughtout badges
        /// </summary>
        /// <param name="templShNumber"></param>
        /// <param name="chooseDate"></param>
        /// <param name="operationCategory"></param>
        /// <param name="shiftPage"></param>
        /// <returns></returns>
        //public IDictionary<short, int> Badges(string templShNumber, DateTime chooseDate, EnumOperationType operationCategory, byte shiftPage = 3) {
        //    return ShippinNumbers.Where(sh =>
        //             sh.n_otpr.StartsWith(templShNumber)
        //                    && (sh.date_oper >= chooseDate.AddDays(-shiftPage) && sh.date_oper <= chooseDate.AddMonths(1).AddDays(shiftPage))
        //                        && ((int)operationCategory == 0 || sh.oper == (int)operationCategory))
        //             .GroupBy(x => new { x.oper })
        //             .Select(g => new { g.Key.oper, operCount = g.Count() })
        //             .ToDictionary(item => item.oper.Value, item => item.operCount);
        //}
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