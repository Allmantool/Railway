﻿using System;
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
using NaftanRailway.BLL.Services.ExpressionTreeExtensions;
using System.Linq.Expressions;
using NaftanRailway.BLL.DTO.General;
using System.Globalization;
using System.Threading.Tasks;
using System.Text;
using System.Data.Entity;
using NaftanRailway.BLL.Services.HierarchyTreeExtensions;

namespace NaftanRailway.BLL.Concrete.BussinesLogic {
    public sealed class NomenclatureModule : Disposable, INomenclatureModule {
        private readonly IBussinesEngage _engage;

        public NomenclatureModule(IBussinesEngage engage) {
            _engage = engage;
        }

        public IEnumerable<T> SkipTable<T>(int page, int initialSizeItem, out long recordCount, Expression<Func<T, bool>> predicate = null) {
            if (predicate != null) {
                //convert type
                var filterPredicate = PredicateExtensions.ConvertTypeExpression<ScrollLineDTO, krt_Naftan>(predicate.Body);
                recordCount = _engage.GetCountRows(filterPredicate);

                return (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollLineDTO>>(_engage.GetSkipRows(page, initialSizeItem, x => x.KEYKRT, filterPredicate));
            }

            recordCount = _engage.GetCountRows<krt_Naftan>(x => true);
            return (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollLineDTO>>(_engage.GetSkipRows<krt_Naftan, long>(page, initialSizeItem, x => x.KEYKRT));
        }

        public IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(long key, IList<CheckListFilter> filters, int page, int initialSizeItem, out long recordCount, bool viewWrong = false) {
            //order predicate
            Expression<Func<krt_Naftan_orc_sapod, object>> order = x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt };
            //filter predicate
            Expression<Func<krt_Naftan_orc_sapod, bool>> where = x => x.keykrt == key;

            //apply filters(linqKit)
            if (filters != null) {
                where = where.And(filters.Aggregate(
                                PredicateBuilder.New<krt_Naftan_orc_sapod>(true).DefaultExpression,
                                (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()))
                        ).Expand();
            }

            //view rows which sum not equal (viewWrong = false)
            where = viewWrong ? where.And(x => x.summa + x.nds != x.sm).Expand() : where;

            recordCount = _engage.GetCountRows(where);

            return Mapper.Map<IEnumerable<ScrollDetailDTO>>(_engage.GetSkipRows(page, initialSizeItem, order, where));
        }

        public ScrollLineDTO GetNomenclatureByNumber(int numberScroll, int reportYear) {
            return Mapper.Map<ScrollLineDTO>(_engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).SingleOrDefault());
        }

        /// <summary>
        /// Get range of available month period
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetListPeriod() {
            var result = _engage.GetGroup<krt_Naftan, DateTime>(x => x.DTBUHOTCHET).Select(x => x.First().DTBUHOTCHET);

            return result;
        }

        /// <summary>
        /// Operation adding information about scroll in table Krt_Naftan_Orc_Sapod and check operation as performed in krt_Naftan
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="msgError"></param>
        /// <param name="numberScroll"></param>
        public IEnumerable<ScrollLineDTO> AddKrtNaftan(int numberScroll, int reportYear, out string msgError) {
            var key = _engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).Select(x => x.KEYKRT).First();
            using (_engage.Uow = new UnitOfWork()) {
                try {
                    SqlParameter parm = new SqlParameter() {
                        ParameterName = "@ErrId",
                        SqlDbType = SqlDbType.TinyInt,
                        Direction = ParameterDirection.Output
                    };
                    //set active context => depend on type of entity
                    var db = _engage.Uow.Repository<krt_Naftan_orc_sapod>().ActiveDbContext.Database;
                    db.CommandTimeout = 120;
                    db.ExecuteSqlCommand(@"EXEC @ErrId = dbo.[sp_fill_krt_Naftan_orc_sapod] @KEYKRT", new SqlParameter("@KEYKRT", key), parm);

                    //Confirmed
                    krt_Naftan chRecord = _engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == key);
                    //_engage.Uow.Repository<krt_Naftan>().Edit(chRecord);

                    //Uow.Repository<krt_Naftan>().Edit(chRecord);
                    if (!chRecord.Confirmed) {
                        chRecord.Confirmed = true;
                        chRecord.CounterVersion = 1;
                    }

                    msgError = "";
                    //write state
                    chRecord.ErrorState = Convert.ToByte((byte)parm.Value);

                    _engage.Uow.Save();

                    return Mapper.Map<IEnumerable<ScrollLineDTO>>(new[] { chRecord });
                } catch (Exception e) {
                    throw new Exception("Failed confirmed data: " + e.Message);
                }
            }
        }

        public ScrollLineDTO DeleteNomenclature(int numberScroll, int reportYear) {
            var key = _engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).Select(x => x.KEYKRT).First();

            using (_engage.Uow = new UnitOfWork()) {
                krt_Naftan chRecord = _engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == key);

                //Cascading delete rows in  krt_Naftan_orc_sapod (set up on .odmx model)
                _engage.Uow.Repository<krt_Naftan>().Delete(chRecord);

                _engage.Uow.Save();

                return Mapper.Map<ScrollLineDTO>(chRecord);
            }

        }

        public async Task<int> SyncWithOrc() {
            using (_engage.Uow = new UnitOfWork()) {
                var result = 0;

                try {
                    var db = _engage.Uow.Repository<krt_Naftan>().ActiveDbContext.Database;
                    db.CommandTimeout = 120;

                    result = await db.ExecuteSqlCommandAsync(TransactionalBehavior.EnsureTransaction, @"EXEC dbo.sp_UpdateKrt_Naftan");
                } catch (Exception ex) {
                    await LogExceptionAsync(ex);
                }

                return result;
            }
        }

        /// <summary>
        /// Change date all later records
        /// </summary>
        /// <param name="period"></param>
        /// <param name="keyScroll"></param>
        /// <param name="multiChange">Change single or multi date</param>
        public IEnumerable<ScrollLineDTO> ChangeBuhDate(DateTime period, long keyScroll, bool multiChange = true) {
            var listRecords = multiChange ? _engage.GetTable<krt_Naftan, int>(x => x.KEYKRT >= keyScroll).ToList() :
                                            _engage.GetTable<krt_Naftan, int>(x => x.KEYKRT == keyScroll).ToList();

            using (_engage.Uow = new UnitOfWork()) {
                try {
                    //add to tracking (for update only change property)
                    _engage.Uow.Repository<krt_Naftan>().Edit(listRecords, x => x.DTBUHOTCHET = period);
                    _engage.Uow.Save();
                    return Mapper.Map<IEnumerable<ScrollLineDTO>>(listRecords);
                } catch (Exception) {
                    throw new Exception("Error on change date method");
                }
            }
        }

        /// <summary>
        /// Edit financial index in selected charge
        /// </summary>
        /// <param name="charge"></param>
        /// <returns></returns>
        public bool EditKrtNaftanOrcSapod(ScrollDetailDTO charge) {
            using (_engage.Uow = new UnitOfWork()) {
                try {
                    //krt_Naftan_ORC_Sapod (check as correction)
                    var itemRow = _engage.Uow.Repository<krt_Naftan_orc_sapod>().Get(x => x.keykrt == charge.keykrt && x.keysbor == charge.keysbor);
                    _engage.Uow.Repository<krt_Naftan_orc_sapod>().Edit(itemRow);

                    //update only necessary properties (exist method whole update method)
                    itemRow.nds = charge.nds;
                    itemRow.summa = charge.summa;
                    itemRow.kol = charge.kol;
                    itemRow.textm = charge.textm;

                    //mark as edit
                    itemRow.ErrorState = 2;

                    //krt_Naftan (check as correction)
                    var parentRow = _engage.Uow.Repository<krt_Naftan>().Get(x => x.KEYKRT == charge.keykrt);
                    _engage.Uow.Repository<krt_Naftan>().Edit(parentRow);
                    //mark as edit
                    parentRow.ErrorState = 2;

                    _engage.Uow.Save();

                } catch (Exception) {
                    return false;
                }

                return true;
            }
        }

        public ScrollDetailDTO OperationOnScrollDetail(long key, EnumMenuOperation operation) {
            var row = Mapper.Map<ScrollDetailDTO>(_engage.GetTable<krt_Naftan_orc_sapod, long>(x => x.keysbor == key, caсhe: true, tracking: true).SingleOrDefault());

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
            //main predicate
            Expression<Func<krt_Naftan_orc_sapod, bool>> predicate = x => x.keykrt == key;

            return new[] {
                new CheckListFilter(_engage.GetGroup<krt_Naftan_orc_sapod, object>(x => new { x.id_kart, x.nkrt }, predicate)
                        .ToDictionary(x=>x.First().id_kart?.ToString(), x=>x.First().nkrt)){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.id_kart),
                    NameDescription = "Карточки:"
                },
                new CheckListFilter(_engage.GetGroup(x => x.tdoc.ToString(), predicate)
                        .ToDictionary( x=>x.First().tdoc.ToString(), x=>x.First().tdoc.ToString())){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.tdoc),
                    NameDescription = "Тип документа:"
                },
                new CheckListFilter(_engage.GetGroup(x => x.vidsbr.ToString(),predicate)
                        .ToDictionary( x=>x.First().vidsbr.ToString(),x=>x.First().vidsbr.ToString() )){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.vidsbr),
                    NameDescription = "Вид сбора:"
                },
                new CheckListFilter(_engage.GetGroup(x => x.nomot.ToString(),predicate)
                        .ToDictionary( x=> x.First().nomot.ToString(), x=> x.First().nomot.ToString())){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.nomot),
                    NameDescription = "Документ:"
                },
                //new CheckListFilter(_engage.GetGroup(x=>x.dt.ToString(CultureInfo.InvariantCulture), predicate).Select(x=>x.ToString())){
                //    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.dt),
                //    NameDescription = "Период:"
                //},
            };
        }

        public IEnumerable<CheckListFilter> initGlobalSearchFilters() {
            CheckListFilter[] result;

            try {
                result = new[] {
                    new CheckListFilter(
                        _engage.GetGroup<krt_Naftan_orc_sapod, object>(  x => new { x.id_kart, x.nkrt }, x=>x.tdoc == 4 && x.id_kart != null, x=> new { x.nkrt }, caсhe: true )
                               .ToDictionary(x=>x.First().id_kart?.ToString(), x=>x.First().nkrt)) {
                        FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod> (x => x.id_kart ),
                        NameDescription = @"Накоп. Карточки"
                    },
                    new CheckListFilter(
                        _engage.GetGroup<krt_Naftan, object>( x => new { x.KEYKRT, x.NKRT }, x => true, x => new { x.NKRT }, caсhe: true )
                            .ToDictionary( x => x.First().KEYKRT.ToString(), x => x.First().NKRT.ToString() )) {
                        FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod> (x=>x.keykrt),
                        NameDescription = @"Перечни"
                    },
                    // new CheckListFilter(
                    //   /* _engage.GetGroup<krt_Naftan_orc_sapod, string>( x => x.num_doc, x => x.num_doc != null && x.num_doc.Length > 0, x => x.num_doc)
                    //        .ToDictionary(x => x.First().num_doc, x => x.First().num_doc)*/) {
                    //    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod> (x => x.num_doc),
                    //    NameDescription = @"Первичный док. (накладные, ведомости, акты, карточки)"
                    //},
            };
            } catch (Exception ex) {
                _engage.Log.Debug($"Method initGlobalSearchFilters throws exception: {ex.Message}."); throw;
            }

            return result;
        }

        /// <summary>
        /// Get report from SSRS (it's I/O operation from remote server, thus i use async method)
        /// The main reason to use 'Tuple' type is async. (it doesn't work with ref and out => i use 'tuple' as workaround)
        /// </summary>
        /// <param name="brInfo"></param>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <param name="serverName"></param>
        /// <param name="folderName"></param>
        /// <param name="reportName"></param>
        /// <param name="defaultParameters"></param>
        /// <returns></returns>
        public async Task<Tuple<byte[], string>> GetNomenclatureReports(BrowserInfoDTO brInfo, int numberScroll, int reportYear, string serverName, string folderName,
                                             string reportName, string defaultParameters = "rs:Format=Excel") {
            string nameFile, filterParameters;
            var selScroll = GetNomenclatureByNumber(numberScroll, reportYear);

            //dictionary name/title file (!Tips: required complex solution in case of scalability)
            switch (reportName) {
                case @"krt_Naftan_Gu12":
                nameFile = string.Format(@"Расшифровка сбора 099 за {0} месяц", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selScroll.DTBUHOTCHET.Month));
                filterParameters = string.Format(@"period={0}", selScroll.DTBUHOTCHET.Date);
                break;

                case @"krt_Naftan_BookkeeperReport":
                nameFile = string.Format(@"Бухгалтерский отчёт по переченю №{0}.xls", numberScroll);
                filterParameters = string.Format(@"nkrt={0}&year={1}", numberScroll, reportYear);
                break;

                case @"krt_Naftan_act_of_Reconciliation":
                nameFile = string.Format(@"Реестр электронного представления перечней ОРЦ за {0} {1} года.xls", selScroll.DTBUHOTCHET.ToString("MMMM"), selScroll.DTBUHOTCHET.Year);
                filterParameters = string.Format(@"month={0}&year={1}", selScroll.DTBUHOTCHET.Month, selScroll.DTBUHOTCHET.Year);
                break;

                case @"KRT_Analys_ORC":
                nameFile = string.Format(@"Отчёт Анализа ЭСЧФ по перечню №{0}.xls", numberScroll);
                filterParameters = string.Format(@"key={0}&startDate={1}", selScroll.KEYKRT, selScroll.DTBUHOTCHET.Date);
                break;

                default:
                nameFile = string.Format(@"Отчёт о ошибках по переченю №{0}.xls", numberScroll);
                filterParameters = string.Format(@"nkrt={0}&year={1}", numberScroll, reportYear);
                break;
            }

            //generate url for ssrs
            string urlReportString = string.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName, folderName, reportName, defaultParameters, filterParameters);

            //Changing "attach;" to "inline;" will cause the file to open in the browser instead of the browser prompting to save the file.
            //encode the filename parameter of Content-Disposition header in HTTP (for support different browser)
            string headersInfo;
            if (brInfo.Name == "IE" && (brInfo.Version == "7.0" || brInfo.Version == "8.0"))
                headersInfo = "attachment; filename=" + Uri.EscapeDataString(nameFile);
            else if (brInfo.Name == "Safari")
                headersInfo = "attachment; filename=" + nameFile;
            else
                headersInfo = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(nameFile);

            //WebClient client = new WebClient { UseDefaultCredentials = true };
            /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
            WebClient client = new WebClient {
                Credentials =
                    new CredentialCache{
                            {new Uri("http://db2"),@"ntlm",new NetworkCredential(@"CPN", @"1111", @"LAN")}
                    }
            };

            byte[] result;
            try {
                //byte output
                result = await client.DownloadDataTaskAsync(urlReportString);
            } catch (Exception ex) {
                //log url
                _engage.Log.DebugFormat($"Attempt to receive report with url: {urlReportString}, but it throws exception: {ex.Message}");
                result = Encoding.ASCII.GetBytes(ex.Message);
            }

            return new Tuple<byte[], string>(result, headersInfo);
        }

        public bool UpdateRelatingFilters(ScrollLineDTO scroll, ref IList<CheckListFilter> filters, EnumTypeFilterMenu typeFilter) {

            if (scroll == null) return false;

            //build lambda expression basic on active filter(linqKit)
            var finalPredicate = filters.Where(x => x.ActiveFilter).Aggregate(
                    PredicateBuilder.New<krt_Naftan_orc_sapod>().And(x => x.keykrt == scroll.KEYKRT),
                    (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>())
                );

            foreach (var item in filters) {
                item.CheckedValues = _engage.GetGroup(PredicateExtensions.GroupPredicate<krt_Naftan_orc_sapod>(item.FieldName).Expand(), finalPredicate.Expand()).Select(x => x.ToString());
            }

            return true;
        }

        public async Task LogExceptionAsync(Exception ex) {
            // Note: this is going to run synchronously
            // because I didn't use async in the body.
            await Task.Run(() => { _engage.Log.Debug($"Exception during execution 'SyncWithOrc': {ex?.Message}"); });
            // Do something async here ...
        }

        protected override void DisposeCore() {
            _engage?.Dispose();
        }

        /// <summary>
        /// It method converts flatted table to node hierarchy structure and return it
        /// </summary>
        /// <returns></returns>
        public async Task<IList<TreeNode>> getTreeStructure() {
            const int countGroup = 20;
            var startPeriod = new DateTime(2017, 1, 1);

            IList<TreeNode> result = new List<TreeNode>();
            IList<TreeNode> tree = new List<TreeNode>();

            var hirearchyDict = new Dictionary<int, string>{
                { 0, "Документ" },
                { 1, "Тип документа" },
                { 3, "Карточка" },
                { 7, "Перечень" },
                { 15, "Сбор" }
            };

            var typeDocDict = new Dictionary<int, string> {
                { 1, "Накладная" },
                { 2, "Ведомость" },
                { 3, "Акт" },
                { 4, "Карточка" },
            };

            #region Query
            /* 04.08.2017
            * It query converts flatted table to hierarchy table (The hierarchy deep is defined by group predicate)
            *
            * [elementId] - primary key
            * [parentId] - parents element id
            * [groupId] - group id
            * [rankInGr] - element primary key in group
            * [treeLevel] - height of tree
            * [levelName] - custom group tree node name
            * [searchkey] - key for search in plane (source table)
            * [label] - description for rendering purpose
            */
            //--Warning weakness!
            //--The order must be same in each aggregation functions
            var query = $@";WITH grSubResult AS (
                SELECT  
                    [elementId] = ROW_NUMBER() OVER(ORDER BY kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [groupId] = DENSE_RANK() OVER(ORDER BY kn.KEYKRT DESC),
                    [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [treeLevel] = GROUPING_ID(kn.KEYKRT, kn.NKRT, knos.id_kart, knos.tdoc, knos.nomot),
                    --[level_card] = GROUPING(knos.id_kart),
                    [scroll] = kn.NKRT, kn.KEYKRT,
                    [card] = knos.NKRT, knos.id_kart,
		            [typeDoc] = knos.tdoc,
		            [docum] = knos.nomot,
                    [count] = COUNT(*)
                FROM [dbo].[krt_Naftan_orc_sapod] AS knos INNER JOIN [dbo].[krt_Naftan] AS kn
                    ON kn.KEYKRT = knos.keykrt
                WHERE knos.tdoc > 0 AND knos.id > 0 AND kn.DTBUHOTCHET >= '{startPeriod:d}'
                GROUP BY GROUPING SETS(
                        --(),
                        (kn.KEYKRT, kn.NKRT),
		                --(kn.KEYKRT, kn.NKRT, knos.id_kart),
                        (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt),
		                (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc),
		                (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc, knos.nomot)
	                )
                )

                SELECT
                    [parentId] = CASE [treeLevel]
					    WHEN 0 THEN MAX(elementId) OVER (PARTITION BY KEYKRT, id_kart, typeDoc)
					    WHEN 1 THEN MAX(elementId) OVER (PARTITION BY KEYKRT, id_kart)
					    WHEN 3 THEN MAX(elementId) OVER (PARTITION BY KEYKRT)
					ELSE 0 END,
	                [elementId], [groupId], [rankInGr], [treeLevel],
	                [levelName] = CASE [treeLevel]
					    WHEN 0 THEN N'Документ'
					    WHEN 1 THEN N'Тип документа'
					    WHEN 3 THEN N'Карточка'
					    WHEN 7 THEN N'Перечень'
					ELSE NULL END,
	                [searchkey] = CASE [treeLevel]
					    WHEN 0 THEN convert(nvarchar(max), [docum])
					    WHEN 1 THEN convert(nvarchar(max), [typeDoc])
					    WHEN 3 THEN convert(nvarchar(max), [id_kart])
					    WHEN 7 THEN convert(nvarchar(max), [keykrt])
					ELSE NULL END,
	                [label] = CASE [treeLevel]
				        WHEN 0 THEN Convert(nvarchar(max), [docum])
				        WHEN 1 THEN Case [typeDoc] when 1 then N'Накладная' when 2 then N'Ведомость' when 3 then N'Акт' Else N'Карточка' End
				        WHEN 3 THEN [card]
				        WHEN 7 THEN CONVERT(NVARCHAR(10),[scroll])
				    ELSE NULL END,
	                [count]
                FROM grSubResult as gr
                WHERE [groupId] <= {countGroup} --  and [treeLevel] IN ( 1, 0)
                ORDER BY KEYKRT DESC, id_kart desc, gr.[typeDoc] desc, [docum] desc;";
            #endregion

            try {
                using (_engage.Uow = new UnitOfWork()) {
                    result = await _engage.Uow.Repository<TreeNode>().ActiveDbContext.Database.SqlQuery<TreeNode>(query).ToListAsync();
                    if (result.Count > 0) tree = result.FillRecursive();
                }
            } catch (Exception ex) {
                _engage.Log.DebugFormat("Exception: {0}", ex.Message);
            }

            return tree;
        }
    }
}