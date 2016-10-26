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
using System.Net;
using System.Web;
using System.Web.Mvc;
using NaftanRailway.BLL.Services.ExpressionTreeExtensions;

namespace NaftanRailway.BLL.Concrete.BussinesLogic {
    public class NomenclatureModule : Disposable, INomenclatureModule {
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

        public IEnumerable<CheckListFilter> InitNomenclatureDetailMenu(long key) {
            return new[] { new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.nkrt,x => x.keykrt ==  key))
                        {FieldName = "nkrt",NameDescription = "Накоп. Карточки:"},
                        new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.tdoc.ToString(),x => x.keykrt ==  key))
                        {FieldName = "tdoc",NameDescription = "Тип документа:"},
                        new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.vidsbr.ToString(),x => x.keykrt ==  key))
                        {FieldName = "vidsbr",NameDescription = "Вид сбора:"},
                        new CheckListFilter(Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.nomot.ToString(),x => x.keykrt == key))
                        {FieldName = "nomot",NameDescription = "Документ:"}
                    };
        }

        public IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(long key, IList<CheckListFilter> filters, int page, byte initialSizeItem, out long recordCount) {
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

        public byte[] GetNomenclatureReports(Controller contr, int numberScroll, int reportYear, string serverName, string folderName, string reportName, string defaultParameters = "rs:Format=Excel") {

            var selScroll = GetNomenclatureByNumber(numberScroll, reportYear);

            string filterParameters = (reportName == @"krt_Naftan_act_of_Reconciliation") ? @"month=" + selScroll.DTBUHOTCHET.Month + @"&year=" + selScroll.DTBUHOTCHET.Year
                : @"nkrt=" + numberScroll + @"&year=" + reportYear;

            string urlReportString = String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName,
                folderName, reportName, defaultParameters, filterParameters);

            //WebClient client = new WebClient { UseDefaultCredentials = true };
            /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
            WebClient client = new WebClient {
                Credentials =
                    new CredentialCache{
                            {new Uri("http://db2"),@"ntlm",new NetworkCredential(@"CPN", @"1111", @"LAN")}
                    }
            };

            string nameFile = (reportName == @"krt_Naftan_BookkeeperReport"
                ? String.Format(@"Бухгалтерский отчёт по переченю №{0}.xls", numberScroll) : (reportName == @"krt_Naftan_act_of_Reconciliation")
                ? String.Format(@"Реестр электронного  представления перечней ОРЦ за {0} {1} года.xls", selScroll.DTBUHOTCHET.ToString("MMMM"), selScroll.DTBUHOTCHET.Year)
                    : String.Format(@"Отчёт о ошибках по переченю №{0}.xls", numberScroll));

            //Changing "attach;" to "inline;" will cause the file to open in the browser instead of the browser prompting to save the file.
            //encode the filename parameter of Content-Disposition header in HTTP (for support diffrent browser)
            string contentDisposition;

            if (contr.Request.Browser.Browser == "IE" && (contr.Request.Browser.Version == "7.0" || contr.Request.Browser.Version == "8.0"))
                contentDisposition = "attachment; filename=" + Uri.EscapeDataString(nameFile);
            else if (contr.Request.Browser.Browser == "Safari")
                contentDisposition = "attachment; filename=" + nameFile;
            else
                contentDisposition = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(nameFile);

            //name file (with encoding)
            contr.Response.AddHeader("Content-Disposition", contentDisposition);

            //For js spinner and complete donwload callback
            contr.Response.Cookies.Clear();
            contr.Response.AppendCookie(new HttpCookie("SSRSfileDownloadToken", "true"));


            return client.DownloadData(urlReportString);
        }

        public bool UpdateRelatingFilters(ScrollLineDTO scroll, ref IList<CheckListFilter> filters, EnumTypeFilterMenu typeFilter) {

            if (scroll == null) return false;

            //build lambda expression basic on active filter(linqKit)
            var finalPredicate = filters.Where(x => x.ActiveFilter).Aggregate(
                    PredicateBuilder.True<krt_Naftan_orc_sapod>().And(x => x.keykrt == scroll.KEYKRT),
                        (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>())
                );

            foreach (var item in filters) {
                item.CheckedValues = Engage.GetGroup(PredicateExtensions.GroupPredicate<krt_Naftan_orc_sapod>(item.FieldName).Expand(), finalPredicate.Expand());
            }

            return true;
        }

        protected override void DisposeCore() {
            if (Engage != null)
                Engage.Dispose();
        }
    }
}