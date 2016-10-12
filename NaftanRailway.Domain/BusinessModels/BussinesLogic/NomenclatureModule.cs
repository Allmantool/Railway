using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    public class NomenclatureModule : INomenclatureModule {
        private bool _disposed;
        public IBussinesEngage Engage { get; set; }
        public NomenclatureModule(IBussinesEngage engage) {
            Engage = engage;
        }

        public IEnumerable<krt_Naftan> SkipScrollTable(int page, int initialSizeItem, out long recordCount) {
            return Engage.GetSkipRows<krt_Naftan, long>(page, initialSizeItem, out recordCount, x => x.KEYKRT);
        }

        /// <summary>
        /// Operation adding information about scroll in table Krt_Naftan_Orc_Sapod and check operation as perfomed in krt_Naftan
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="msgError"></param>
        /// <param name="numberScroll"></param>
        public krt_Naftan AddKrtNaftan(int numberScroll, int reportYear, out string msgError) {
            var key = Engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).Select(x => x.KEYKRT).First();
            using (Engage.Uow = new UnitOfWork()) {
                try {
                    SqlParameter parm = new SqlParameter() {
                        ParameterName = "@ErrId",
                        SqlDbType = SqlDbType.TinyInt,
                        Direction = ParameterDirection.Output
                    };
                    //set active context => depend on type of entity
                    var db = Engage.Uow.Repository<krt_Naftan_orc_sapod>().ActiveContext.Database;
                    db.CommandTimeout = 120;
                    db.ExecuteSqlCommand(@"EXEC @ErrId = dbo.[sp_fill_krt_Naftan_orc_sapod] @KEYKRT", new SqlParameter("@KEYKRT", key), parm);

                    //Confirmed
                    krt_Naftan chRecord = Engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == key);
                    //Engage.Uow.Repository<krt_Naftan>().Edit(chRecord);

                    //Uow.Repository<krt_Naftan>().Edit(chRecord);
                    if (!chRecord.Confirmed) {
                        chRecord.Confirmed = true;
                        chRecord.CounterVersion = 1;
                    }

                    msgError = "";
                    chRecord.ErrorState = Convert.ToByte((byte)parm.Value);

                    Engage.Uow.Save();

                    return chRecord;
                } catch (Exception e) {
                    throw new Exception("Failed confirmed data: " + e.Message);
                }
            }
        }

        public void SyncWithOrc() {
            using (Engage.Uow = new UnitOfWork()) {
                var db = Engage.Uow.Repository<krt_Naftan>().ActiveContext.Database;
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
            var listRecords = multiChange ? Engage.GetTable<krt_Naftan, int>(x => x.NKRT >= numberScroll && x.DTBUHOTCHET.Year == period.Year).ToList() :
                        Engage.GetTable<krt_Naftan, int>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == period.Year).ToList();

            using (Engage.Uow = new UnitOfWork()) {
                try {
                    //add to tracking (for apdate only change property)
                    Engage.Uow.Repository<krt_Naftan>().Edit(listRecords, x => x.DTBUHOTCHET = period);
                    Engage.Uow.Save();
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
            using (Engage.Uow = new UnitOfWork()) {
                try {
                    //krt_Naftan_ORC_Sapod (check as correction)
                    var itemRow = Engage.Uow.Repository<krt_Naftan_orc_sapod>().Get(x => x.keykrt == keykrt && x.keysbor == keysbor);
                    Engage.Uow.Repository<krt_Naftan_orc_sapod>().Edit(itemRow);
                    itemRow.nds = nds;
                    itemRow.summa = summa;
                    itemRow.ErrorState = 2;

                    //krt_Naftan (check as correction)
                    var parentRow = Engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == keykrt);
                    Engage.Uow.Repository<krt_Naftan>().Edit(parentRow);

                    parentRow.ErrorState = 2;

                    Engage.Uow.Save();

                } catch (Exception) {
                    return false;
                }

                return true;
            }
        }

        public krt_Naftan_orc_sapod OperationOnScrollDetail(long key, EnumMenuOperation operation) {
            var row = Engage.GetTable<krt_Naftan_orc_sapod, long>(x => x.keysbor == key, caсhe: true, tracking: true).SingleOrDefault();

            switch (operation) {
                case EnumMenuOperation.Join:
                return row;
                case EnumMenuOperation.Edit:
                return row;
                case EnumMenuOperation.Delete:
                return row;
                default:
                return row;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Engage.Dispose();
                }
                _disposed = true;
            }
        }
    }
}