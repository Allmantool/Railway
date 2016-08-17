using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using MoreLinq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    public class NomenclatureModule : INomenclatureModule {
        private bool _disposed;
        public IBussinesEngage _engage { get; set; }
        public NomenclatureModule(IBussinesEngage engage) {
            _engage = engage;
        }

        //public T DetailsData<T>(int numberScroll, int reportYear, int page) where T :class, new()
        //{

        //}
        public IEnumerable<krt_Naftan> SkipScrollTable(int page, int initialSizeItem, out long recordCount) {
            return _engage.GetSkipRows<krt_Naftan, long>(page, initialSizeItem, out recordCount, x => x.KEYKRT);
        }
        /// <summary>
        /// Operation adding information about scroll in table Krt_Naftan_Orc_Sapod and check operation as perfomed in krt_Naftan
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="msgError"></param>
        /// <param name="numberScroll"></param>
        public krt_Naftan AddKrtNaftan(int numberScroll, int reportYear, out string msgError) {
            var key = _engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).Select(x => x.KEYKRT).First();
            using (_engage.Uow = new UnitOfWork()) {
                try {
                    SqlParameter parm = new SqlParameter() {
                        ParameterName = "@ErrId",
                        SqlDbType = SqlDbType.TinyInt,
                        Direction = ParameterDirection.Output
                    };
                    //set active context => depend on type of entity
                    var db = _engage.Uow.Repository<krt_Naftan_orc_sapod>().Context.Database;
                    db.CommandTimeout = 120;
                    db.ExecuteSqlCommand(@"EXEC @ErrId = dbo.sp_fill_krt_Naftan_orc_sapod @KEYKRT", new SqlParameter("@KEYKRT", key), parm);

                    //Confirmed
                    krt_Naftan chRecord = _engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == key);

                    //Uow.Repository<krt_Naftan>().Edit(chRecord);
                    if (!chRecord.Confirmed) {
                        chRecord.Confirmed = true;
                        chRecord.CounterVersion = 1;
                    }

                    msgError = "";
                    chRecord.ErrorState = Convert.ToByte((byte)parm.Value);

                    _engage.Uow.Save();

                    return chRecord;
                } catch (Exception e) {
                    msgError = e.Message;
                    throw new Exception("Failed confirmed data");
                }
            }
        }
        public void SyncWithORC() {
            using (_engage.Uow = new UnitOfWork()) {
                var db = _engage.Uow.Repository<krt_Naftan>().Context.Database;
                db.CommandTimeout = 120;
                db.ExecuteSqlCommand(@"EXEC dbo.sp_UpdateKrt_Naftan");
            }
        }
        /// <summary>
        /// Change date all later records
        /// </summary>
        /// <param name="period"></param>
        /// <param name="numberScroll"></param>
        /// <param name="multiChange">Change single or multi date</param>
        public IEnumerable<krt_Naftan> ChangeBuhDate(DateTime period, int numberScroll, bool multiChange = true) {
            using (_engage.Uow = new UnitOfWork()) {
                var listRecords = multiChange ? _engage.Uow.Repository<krt_Naftan>().Get_all(x => x.NKRT >= numberScroll && x.DTBUHOTCHET.Year == period.Year) :
                    _engage.Uow.Repository<krt_Naftan>().Get_all(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == period.Year);

                try {
                    listRecords.ForEach(x => x.DTBUHOTCHET = period);
                    _engage.Uow.Save();
                    return listRecords;
                } catch (Exception) {
                    throw new Exception("Error on change date method");
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
            using (_engage.Uow = new UnitOfWork()) {
                try {
                    //krt_Naftan_ORC_Sapod (check as correction)
                    var itemRow = _engage.Uow.Repository<krt_Naftan_orc_sapod>().Get(x => x.keykrt == keykrt && x.keysbor == keysbor);
                    _engage.Uow.Repository<krt_Naftan_orc_sapod>().Edit(itemRow);
                    itemRow.nds = nds;
                    itemRow.summa = summa;
                    itemRow.ErrorState = 2;

                    //krt_Naftan (check as correction)
                    var parentRow = _engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == keykrt);
                    _engage.Uow.Repository<krt_Naftan>().Edit(parentRow);

                    parentRow.ErrorState = 2;

                    _engage.Uow.Save();

                } catch (Exception) {
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
                if (disposing) {
                    _engage.Dispose();
                }
                _disposed = true;
            }
        }
    }
}