using System;
using AutoMapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;
using LinqKit;

namespace NaftanRailway.BLL.Concrete {
    public class NomenclatureModule : INomenclatureModule {
        private bool _disposed;
        public IBussinesEngage Engage { get; set; }
        public NomenclatureModule(IBussinesEngage engage) {
            Engage = engage;
        }

        public IEnumerable<T> SkipTable<T>(int page, int initialSizeItem, out long recordCount) {

            var @switch = new Dictionary<Type, IEnumerable<T>> {
                { typeof(ScrollLineDTO), (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollLineDTO>>(Engage.GetSkipRows<krt_Naftan, long>(page, initialSizeItem, out recordCount, x => x.KEYKRT))},
                { typeof(ScrollDetailDTO), (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollDetailDTO>>(Engage.GetSkipRows<krt_Naftan_orc_sapod, long>(page, initialSizeItem, out recordCount, x => x.keykrt)) },
            };

            return @switch[typeof(T)];
        }
        public IEnumerable<T> SkipTable<T>(long key, int page, int initialSizeItem) {

            var @switch = new Dictionary<Type, IEnumerable<T>> {
                { typeof(ScrollLineDTO), (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollLineDTO>>(Engage.GetSkipRows<krt_Naftan, long>(page, initialSizeItem, null, x => x.KEYKRT == key))},
                { typeof(ScrollDetailDTO), (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollDetailDTO>>(Engage.GetSkipRows<krt_Naftan_orc_sapod, object>(page, initialSizeItem, x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt }, x => x.keykrt == key)) },
            };

            return @switch[typeof(T)];
        }

        public ScrollLineDTO GetNomenclatureByNumber(int numberScroll, int reportYear) {
            return Mapper.Map<ScrollLineDTO>(Engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).SingleOrDefault());
        }


        /// <summary>
        /// Operation adding information about scroll in table Krt_Naftan_Orc_Sapod and check operation as perfomed in krt_Naftan
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="msgError"></param>
        /// <param name="numberScroll"></param>
        public IEnumerable<ScrollLineDTO> AddKrtNaftan(int numberScroll, int reportYear, out string msgError) {
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

                    return Mapper.Map<IEnumerable<ScrollLineDTO>>(chRecord);
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
        public IEnumerable<ScrollLineDTO> ChangeBuhDate(DateTime period, int numberScroll, bool multiChange = true) {
            var listRecords = multiChange ? Engage.GetTable<krt_Naftan, int>(x => x.NKRT >= numberScroll && x.DTBUHOTCHET.Year == period.Year).ToList() :
                        Engage.GetTable<krt_Naftan, int>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == period.Year).ToList();

            using (Engage.Uow = new UnitOfWork()) {
                try {
                    //add to tracking (for apdate only change property)
                    Engage.Uow.Repository<krt_Naftan>().Edit(listRecords, x => x.DTBUHOTCHET = period);
                    Engage.Uow.Save();
                    return Mapper.Map<IEnumerable<ScrollLineDTO>>(listRecords);
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

        public ScrollDetailDTO OperationOnScrollDetail(long key, EnumMenuOperation operation) {
            var row = Mapper.Map<ScrollDetailDTO>(Engage.GetTable<krt_Naftan_orc_sapod, long>(x => x.keysbor == key, caсhe: true, tracking: true).SingleOrDefault());

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

        public IEnumerable<CheckListFilter> InitNomenclatureDetailMenu(long key) {
            return new[] { new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.nkrt,x => x.keykrt ==  key))
                        {SortFieldName = "nkrt",NameDescription = "Накоп. Карточки:"},
                        new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.tdoc.ToString(),x => x.keykrt ==  key))
                        {SortFieldName = "tdoc",NameDescription = "Тип документа:"},
                        new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.vidsbr.ToString(),x => x.keykrt ==  key))
                        {SortFieldName = "vidsbr",NameDescription = "Вид сбора:"},
                        new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.nomot.ToString(),x => x.keykrt == key))
                        {SortFieldName = "nomot",NameDescription = "Документ:"}
                    };
        }

        public IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(long key,IList<CheckListFilter> filters, int page, byte initialSizeItem, out long recordCount) {
            //upply filters(linqKit)
            var finalPredicate = filters.Aggregate(PredicateBuilder.True<krt_Naftan_orc_sapod>()
                .And(x => x.keykrt == key), (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()));

            var srcRows = Engage.GetSkipRows<krt_Naftan_orc_sapod, object>(page, initialSizeItem, out recordCount,
                   x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt }, finalPredicate.Expand()).ToList();

            return Mapper.Map<IEnumerable<ScrollDetailDTO>>(srcRows);
        }

        public IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(IList<CheckListFilter> filters, int page, byte initialSizeItem, out long recordCount) {
            throw new NotImplementedException();
        }
    }
}