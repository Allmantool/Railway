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
        public IEnumerable<Shipping> ShippingsViews(EnumOperationType operationCategory, DateTime chooseDate, int page, int pageSize, out int recordCount) {
            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together, resolve through expression tree maybe)
            var cashSrc = Uow.Repository<krt_Guild18>().Get_all(x => x.reportPeriod == chooseDate, false)
                .GroupBy(x => new { x.reportPeriod, x.idDeliviryNote, x.warehouse })
                .OrderBy(x => x.Key.idDeliviryNote).Skip(pageSize * (page - 1)).Take(pageSize).ToList();
            //linqkit
            var votprPredicate = PredicateBuilder.False<v_otpr>();
            votprPredicate = cashSrc.Select(x => x.Key.idDeliviryNote).Aggregate(votprPredicate, (current, value) => current.Or(e => e.id == value)).Expand();
            var cashSrc2 = Uow.Repository<v_otpr>().Get_all(votprPredicate, false).ToList();

            var vovPredicate = PredicateBuilder.False<v_o_v>();
            vovPredicate = cashSrc2.Select(x => x.id).Aggregate(vovPredicate, (current, value) => current.Or(v => v.id_otpr == value)).Expand();
            var cashSrc3 = Uow.Repository<v_o_v>().Get_all(vovPredicate, false).ToList();

            var result = (from kg in cashSrc join vo in cashSrc2 on kg.Key.idDeliviryNote equals vo.id into g1
                          from item in g1.DefaultIfEmpty()
                          join e in Uow.Repository<etsng>().Get_all(enablecaching: false) on item == null ? "" : item.cod_tvk_etsng equals e.etsng1 into g2
                          from item2 in g2.DefaultIfEmpty()
                          select new Shipping() {
                              VOtpr = item,
                              Vovs = cashSrc3.Where(x => (x != null) && x.id_otpr == item.id),
                              VPams = Uow.ActiveContext.Database.SqlQuery<v_pam>(@"
                                select distinct vp.id_ved,vp.nved,vp.dzakr,vp.id_kart,vp.nkrt 
                                from [db2].[nsd2].[dbo].krt_Guild18 as kg inner join [obd].[dbo].v_pam as vp 
                                    on kg.idSrcDocument = vp.id_ved 
                                where kg.type_doc = 2 and vp.[state] = 32 and vp.[kodkl] IN (N'3494',N'349402') 
                                    and kg.reportPeriod = @param1 and kg.idDeliviryNote = @param2",
                                 new SqlParameter("@param1", chooseDate),
                                 new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),
                              VAkts = Uow.ActiveContext.Database.SqlQuery<v_akt>(@"
                                select distinct nakt,dakt,nkrt,id_kart
                                from [db2].[nsd2].[dbo].[krt_Guild18] as kg inner join [obd].[dbo].v_akt as va 
                                    on kg.idSrcDocument  = va.id
                                where kg.type_doc = 3 and va.[state] = 32 and va.kodkl in (N'3494',N'349402')
                                    and kg.reportPeriod = @param1 and kg.idDeliviryNote = @param2",
                              new SqlParameter("@param1", chooseDate),
                              new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),
                              VKarts = Uow.ActiveContext.Database.SqlQuery<v_kart>(@"
                                select distinct vk.id,vk.num_kart,vk.date_okrt,vk.summa,vk.date_fdu93,vk.date_zkrt
                                from [db2].[nsd2].[dbo].[krt_Guild18] as kg inner join [obd].[dbo].v_kart as vk
                                    on kg.idCard  = vk.id
                                where vk.cod_pl in (N'3494',N'349402') and kg.reportPeriod = @param1 and kg.idDeliviryNote = @param2",
                              new SqlParameter("@param1", chooseDate),
                              new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),
                              KNaftan = Uow.ActiveContext.Database.SqlQuery<krt_Naftan>(@"
                                select distinct kg.idDeliviryNote, KEYKRT,nkrt,NTREB,DTBUHOTCHET,DTBUHOTCHET,DTOPEN,SMTREB,NDSTREB,P_TYPE,RecordCount,Scroll_Sbor
                                from [db2].[nsd2].[dbo].[krt_Guild18] as kg inner join [db2].[nsd2].[dbo].[krt_Naftan] AS kn 
                                    on kg.idScroll = kn.KEYKRT
                                where kg.reportPeriod = @param1 and kg.idDeliviryNote = @param2",
                                new SqlParameter("@param1", chooseDate),
                                new SqlParameter("@param2", item != null ? item.id : 0)).ToList(),
                              Etsng = item2,
                              Guild18 = new krt_Guild18 {
                                  reportPeriod = kg.Key.reportPeriod,
                                  idDeliviryNote = kg.Key.idDeliviryNote,
                                  warehouse = kg.Key.warehouse
                              }
                          }).Where(x => ((x.VOtpr != null) && x.VOtpr.oper == (short)operationCategory) || operationCategory == EnumOperationType.All).ToList();


            recordCount = result.Count();
            return result.ToList();
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
            var endDate = chooseDate.AddDays(shiftPage);

            return GetGroup<v_otpr, string>(x => new { x.id, x.n_otpr }.n_otpr, x => x.n_otpr.StartsWith(templShNumber)
                && (x.date_oper >= startDate && x.date_oper <= endDate)).OrderByDescending(x => x).Take(20);
        }
        public ShippingInfoLine PackDocuments(v_otpr shipping, int warehouse) {
            throw new NotImplementedException();
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
        public IEnumerable<T> GetTable<T, TKey>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TKey>> orderPredicate = null) where T : class {
            using (Uow = new UnitOfWork()) {
                return (orderPredicate == null) ? Uow.Repository<T>().Get_all(predicate).ToList() : Uow.Repository<T>().Get_all(predicate).OrderByDescending(orderPredicate).ToList();
            }
        }
        public long GetCountRows<T>(Expression<Func<T, bool>> predicate = null) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate, false).Count();
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
        public IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(filterPredicate, false).OrderByDescending(orderPredicate).Skip((page - 1) * size).Take(size).ToList();
            }
        }
        public IEnumerable<TKey> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> predicate = null) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate, false).GroupBy(groupPredicate).OrderBy(x => x.Key).Select(x => x.Key).ToList();
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