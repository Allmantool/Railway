using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using MoreLinq;
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
        /// Формирования объекта отображения информации об отправках
        /// </summary>
        /// <param name="templShNumber">Regular expression for searched filter</param>
        /// <param name="operationCategory">filter on category</param>
        /// <param name="chooseDate">Work period</param>
        /// <param name="page">Current page</param>
        /// <param name="shiftDate">correction number</param>
        /// <param name="pageSize">Count item on page</param>
        /// <returns></returns>
        public IEnumerable<Shipping> ShippingsViews(string templShNumber, EnumOperationType operationCategory, DateTime chooseDate, int page = 1, int pageSize = 8) {
            //linq to object(etsng) copy in memory (because EF don't support two dbcontext work together, resolve through expression tree)

            var sub1 = GetSkipRows<krt_Guild18, int>(page, pageSize, x => x.recordNumber, x => x.reportPeriod == chooseDate);
            //var sub2 = GetTable<v_otpr, int>(x => sub1.Contains());
            var result = from kg in Uow.Repository<krt_Guild18>().Get_all(x => x.reportPeriod == chooseDate).ToList()
                         join vo in Uow.Repository<v_otpr>().Get_all(x => x.state == 32).AsExpandable() on kg.idDeliviryNote equals vo.id into g1
                         from j1 in g1.DefaultIfEmpty()
                         //join e in Uow.Repository<etsng>().Get_all().AsExpandable() on j1.cod_tvk_etsng equals e.etsng1 into g2
                         //from j2 in g2.DefaultIfEmpty()
                         select new Shipping(){
                             //VOtpr = j1,
                             Guild18 = kg,
                             //Etsng = j2
                         };

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
                return Uow.Repository<T>().Get_all(predicate).Count();
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
                return Uow.Repository<T>().Get_all(filterPredicate).OrderByDescending(orderPredicate).Skip((page - 1) * size).Take(size).ToList();
            }
        }
        public IEnumerable<TKey> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> predicate = null) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate).GroupBy(groupPredicate).OrderBy(x=>x.Key).Select(x => x.Key).ToList();
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
                    Uow.Repository<krt_Naftan_orc_sapod>()._context.Database.CommandTimeout = 120;
                    Uow.Repository<krt_Naftan_orc_sapod>()._context.Database.ExecuteSqlCommand(@"EXEC @ErrId = dbo.sp_fill_krt_Naftan_orc_sapod @KEYKRT", new SqlParameter("@KEYKRT", key), parm);
                    //alternative, get gropu of entities and then  save in db
                    //this.Database.SqlQuery<YourEntityType>("storedProcedureName",params);

                    //Confirmed
                    krt_Naftan chRecord = Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == key);

                    Uow.Repository<krt_Naftan>().Edit(chRecord);
                    if (!chRecord.Confirmed){
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