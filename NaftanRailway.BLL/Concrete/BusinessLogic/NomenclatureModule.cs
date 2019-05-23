using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.General;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.Services.ExpressionTreeExtensions;
using NaftanRailway.BLL.Services.HierarchyTreeExtensions;
using Railway.Core;
using Railway.Core.Data.EF;
using Railway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.BLL.Concrete.BusinessLogic
{
    public sealed class NomenclatureModule : Disposable, INomenclatureModule
    {
        private readonly IBusinessProvider _provider;

        public NomenclatureModule(IBusinessProvider provider)
        {
            this._provider = provider;
        }

        public IEnumerable<T> SkipTable<T>(
            int page, int initialSizeItem,
            out long recordCount,
            Expression<Func<T, bool>> predicate = null)
        {
            if (predicate != null)
            {
                var filterPredicate = PredicateExtensions.ConvertTypeExpression<ScrollLineDTO, krt_Naftan>(predicate.Body);
                recordCount = this._provider.GetCountRows(filterPredicate);

                return (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollLineDTO>>(this._provider.GetSkipRows(page, initialSizeItem, x => x.KEYKRT, filterPredicate));
            }

            recordCount = this._provider.GetCountRows<krt_Naftan>(x => true);
            return (IEnumerable<T>)Mapper.Map<IEnumerable<ScrollLineDTO>>(this._provider.GetSkipRows<krt_Naftan, long>(page, initialSizeItem, x => x.KEYKRT));
        }

        public IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(
            long key,
            IList<CheckListFilter> filters,
            int page,
            int initialSizeItem,
            out long recordCount,
            bool viewWrong = false)
        {
            Expression<Func<krt_Naftan_orc_sapod, object>> order = x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt };
            Expression<Func<krt_Naftan_orc_sapod, bool>> where = x => x.keykrt == key;

            if (filters != null)
            {
                where = where.And(filters.Aggregate(
                                PredicateBuilder.New<krt_Naftan_orc_sapod>(true).DefaultExpression,
                                (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()))
                        ).Expand();
            }

            where = viewWrong ? where.And(x => x.summa + x.nds != x.sm).Expand() : where;

            recordCount = this._provider.GetCountRows(where);

            return Mapper.Map<IEnumerable<ScrollDetailDTO>>(this._provider.GetSkipRows(page, initialSizeItem, order, where));
        }

        public ScrollLineDTO GetNomenclatureByNumber(int numberScroll, int reportYear)
        {
            return Mapper.Map<ScrollLineDTO>(this._provider.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).SingleOrDefault());
        }

        /// <summary>
        /// Get range of available month period
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetListPeriod()
        {
            var result = this._provider.GetGroup<krt_Naftan, DateTime>(x => x.DTBUHOTCHET).Select(x => x.First().DTBUHOTCHET);

            return result;
        }

        /// <summary>
        /// Operation adding information about scroll in table Krt_Naftan_Orc_Sapod and check operation as performed in krt_Naftan
        /// </summary>
        public IEnumerable<ScrollLineDTO> AddKrtNaftan(int numberScroll, int reportYear, out string msgError)
        {
            var key = this._provider.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear)
                .Select(x => x.KEYKRT)
                .SingleOrDefault();

            using (this._provider.Uow = new UnitOfWork())
            {
                try
                {
                    var sqlParams = new[]
                    {
                        new SqlParameter
                        {
                            ParameterName = "@KEYKRT",
                            SqlDbType = SqlDbType.Float,
                            Direction = ParameterDirection.Input,
                            Value = key
                        },
                        new SqlParameter
                        {
                            ParameterName = "@ErrId",
                            SqlDbType = SqlDbType.TinyInt,
                            Direction = ParameterDirection.Output
                        }
                    };

                    this._provider.Uow.GetRepository<krt_Naftan_orc_sapod>().ExecuteSql(
                        @"EXEC @ErrId = dbo.[sp_fill_krt_Naftan_orc_sapod] @KEYKRT",
                        sqlParams
                    );

                    var chRecord = this._provider.Uow.GetRepository<krt_Naftan>().Get(x => x.KEYKRT == key);

                    if (!chRecord.Confirmed)
                    {
                        chRecord.Confirmed = true;
                        chRecord.CounterVersion = 1;
                    }

                    msgError = "";
                    chRecord.ErrorState = Convert.ToByte((byte)sqlParams.SingleOrDefault(p => string.Equals(p.ParameterName, "@ErrId"))?.Value);

                    this._provider.Uow.Save();

                    return Mapper.Map<IEnumerable<ScrollLineDTO>>(new[] { chRecord });
                }
                catch (Exception e)
                {
                    throw new Exception("Failed confirmed data: " + e.Message);
                }
            }
        }

        public ScrollLineDTO DeleteNomenclature(int numberScroll, int reportYear)
        {
            var key = this._provider.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).Select(x => x.KEYKRT).First();

            using (this._provider.Uow = new UnitOfWork())
            {
                var chRecord = this._provider.Uow.GetRepository<krt_Naftan>().Get(x => x.KEYKRT == key);

                //Cascading delete rows in  krt_Naftan_orc_sapod (set up on .edmx model)
                this._provider.Uow.GetRepository<krt_Naftan>().Delete(chRecord);

                this._provider.Uow.Save();

                return Mapper.Map<ScrollLineDTO>(chRecord);
            }

        }

        public async Task<int> SyncWithOrc()
        {
            using (this._provider.Uow = new UnitOfWork())
            {

                var result = 0;

                try
                {
                    result = await this._provider.Uow.GetRepository<krt_Naftan>().ExecuteSqlAsync(@"EXEC dbo.sp_UpdateKrt_Naftan");
                }
                catch (Exception ex)
                {
                    await this.LogExceptionAsync(ex);
                }

                return result;
            }
        }

        public IEnumerable<ScrollLineDTO> ChangeBuhDate(DateTime period, long keyScroll, bool multiChange = true)
        {
            using (this._provider.Uow = new UnitOfWork())
            {
                try
                {
                    var listRecords = multiChange
                    ? this._provider.Uow.GetRepository<krt_Naftan>().GetAll(x => x.KEYKRT >= keyScroll)
                                          : this._provider.Uow.GetRepository<krt_Naftan>().GetAll(x => x.KEYKRT == keyScroll);

                    this._provider.Uow.GetRepository<krt_Naftan>()
                    .Update(
                            listRecords,
                            new List<Action<krt_Naftan>> { x => x.DTBUHOTCHET = period });

                    this._provider.Uow.Save();

                    return Mapper.Map<IEnumerable<ScrollLineDTO>>(listRecords);
                }
                catch (Exception)
                {
                    throw new Exception("Error on change date method");
                }
            }
        }

        /// <summary>
        /// Edit financial index in selected charge
        /// </summary>
        /// <param name="charge"></param>
        /// <returns></returns>
        public bool EditKrtNaftanOrcSapod(ScrollDetailDTO charge)
        {
            try
            {
                using (this._provider.Uow = new UnitOfWork())
                {
                    var conditions = new List<Action<krt_Naftan_orc_sapod>>
                                         {
                                             x => x.nds = charge.nds,
                                             x => x.summa = charge.summa,
                                             x => x.kol = charge.kol,
                                             x => x.textm = charge.textm,
                                             x => x.ErrorState = 2
                                         };

                    this._provider.Uow.GetRepository<krt_Naftan_orc_sapod>()
                    .Update(
                        x => x.keykrt == charge.keykrt && x.keysbor == charge.keysbor,
                        conditions);

                    this._provider.Uow.GetRepository<krt_Naftan>().Update(
                        x => x.KEYKRT == charge.keykrt,
                        new List<Action<krt_Naftan>> { x => x.ErrorState = 2 });

                    this._provider.Uow.Save();
                }
            }
            catch (Exception ex)
            {
                this._provider.Log.Debug($"Method EditKrtNaftanOrcSapod throws exception: {ex.Message}.");

                return false;
            }

            return true;
        }

        public ScrollDetailDTO OperationOnScrollDetail(long key, EnumMenuOperation operation)
        {
            var row = Mapper.Map<ScrollDetailDTO>(this._provider.GetTable<krt_Naftan_orc_sapod, long>(x => x.keysbor == key, cache: true, tracking: true).SingleOrDefault());

            switch (operation)
            {
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

        public IEnumerable<CheckListFilter> InitNomenclatureDetailMenu(long key)
        {
            //main predicate
            Expression<Func<krt_Naftan_orc_sapod, bool>> predicate = x => x.keykrt == key;

            return new[] {
                new CheckListFilter(this._provider.GetGroup<krt_Naftan_orc_sapod, object>(x => new { x.id_kart, x.nkrt }, predicate)
                    .ToDictionary(x=>x.First().id_kart?.ToString(), x=>x.First().nkrt)){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.id_kart),
                    NameDescription = "Карточки:"
                },
                    new CheckListFilter(this._provider.GetGroup(x => x.tdoc.ToString(), predicate)
                    .ToDictionary( x=>x.First().tdoc.ToString(), x=>x.First().tdoc.ToString())){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.tdoc),
                    NameDescription = "Тип документа:"
                },
                    new CheckListFilter(this._provider.GetGroup(x => x.vidsbr.ToString(),predicate)
                    .ToDictionary( x=>x.First().vidsbr.ToString(),x=>x.First().vidsbr.ToString() )){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.vidsbr),
                    NameDescription = "Вид сбора:"
                },
                    new CheckListFilter(this._provider.GetGroup(x => x.nomot.ToString(),predicate)
                    .ToDictionary( x=> x.First().nomot.ToString(), x=> x.First().nomot.ToString())){
                    FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x=>x.nomot),
                    NameDescription = "Документ:"
                }
            };
        }

        public IEnumerable<CheckListFilter> InitGlobalSearchFilters()
        {
            CheckListFilter[] result;

            try
            {
                result = new[] {
                    new CheckListFilter(
                    this._provider.GetGroup<krt_Naftan_orc_sapod, object>(  x => new { x.id_kart, x.nkrt }, x=>x.tdoc == 4 && x.id_kart != null, x=> new { x.nkrt }, cache: true )
                            .ToDictionary(x=>x.First().id_kart?.ToString(), x=>x.First().nkrt)) {
                        FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod> (x => x.id_kart ),
                        NameDescription = @"Накоп. Карточки"
                    },
                    new CheckListFilter(
                    this._provider.GetGroup<krt_Naftan, object>( x => new { x.KEYKRT, x.NKRT }, x => true, x => new { x.NKRT }, cache: true )
                        .ToDictionary( x => x.First().KEYKRT.ToString(), x => x.First().NKRT.ToString() )) {
                        FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod> (x=>x.keykrt),
                        NameDescription = @"Перечни"
                    }
                };
            }
            catch (Exception ex)
            {
                this._provider.Log.Debug($"Method initGlobalSearchFilters throws exception: {ex.Message}."); throw;
            }

            return result;
        }

        /// <summary>
        /// Get report from SSRS (it's I/O operation from remote server, thus i use async method)
        /// The main reason to use 'Tuple' type is async. (it doesn't work with ref and out => i use 'tuple' as workaround)
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<byte[], string>> GetNomenclatureReports(BrowserInfoDTO brInfo, int numberScroll, int reportYear, string serverName, string folderName,
                                             string reportName, string defaultParameters = "rs:Format=Excel")
        {
            string nameFile, filterParameters;
            var selScroll = this.GetNomenclatureByNumber(numberScroll, reportYear);

            //dictionary name/title file (!Tips: required complex solution in case of scalability)
            switch (reportName)
            {
                case @"krt_Naftan_Gu12":
                    nameFile = string.Format(@"Расшифровка сбора 099 за {0} месяц", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selScroll.DTBUHOTCHET.Month));
                    filterParameters = $@"period={selScroll.DTBUHOTCHET.Date}";
                    break;

                case @"krt_Naftan_BookkeeperReport":
                    nameFile = $@"Бухгалтерский отчёт по переченю №{numberScroll}.xls";
                    filterParameters = $@"nkrt={numberScroll}&year={reportYear}";
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
            WebClient client = new WebClient
            {
                Credentials =
                    new CredentialCache{
                            {new Uri("http://db2"),@"ntlm",new NetworkCredential(@"CPN", @"1111", @"LAN")}
                    }
            };

            byte[] result;
            try
            {
                //byte output
                result = await client.DownloadDataTaskAsync(urlReportString);
            }
            catch (Exception ex)
            {
                this._provider.Log.DebugFormat($"Attempt to receive report with url: {urlReportString}, but it throws exception: {ex.Message}");
                result = Encoding.ASCII.GetBytes(ex.Message);
            }

            return new Tuple<byte[], string>(result, headersInfo);
        }

        public bool UpdateRelatingFilters(ScrollLineDTO scroll, ref IList<CheckListFilter> filters, EnumTypeFilterMenu typeFilter)
        {

            if (scroll == null) return false;

            //build lambda expression basic on active filter(linqKit)
            var finalPredicate = filters.Where(x => x.ActiveFilter).Aggregate(
                    PredicateBuilder.New<krt_Naftan_orc_sapod>().And(x => x.keykrt == scroll.KEYKRT),
                    (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>())
                );

            foreach (var item in filters)
            {
                item.CheckedValues = this._provider.GetGroup(PredicateExtensions.GroupPredicate<krt_Naftan_orc_sapod>(item.FieldName).Expand(), finalPredicate.Expand()).Select(x => x.ToString());
            }

            return true;
        }

        public async Task LogExceptionAsync(Exception ex)
        {
            // Note: this is going to run synchronously
            // because I didn't use async in the body.
            await Task.Run(() => { this._provider.Log.Debug($"Exception during execution 'SyncWithOrc': {ex?.Message}"); });
            // Do something async here ...
        }

        protected override void ExtenstionDispose()
        {
            this._provider?.Dispose();
        }

        /// <summary>
        /// It method converts flatted table to node hierarchy structure and return it
        /// </summary>
        /// <returns></returns>
        public IList<TreeNode> GetTreeStructure(int typeDoc = 63, string rootKey = null)
        {
            var hierarchyDict = new Dictionary<int, string>{
                { 0, "Документ" },
                { 1, "Тип документа" },
                { 3, "Карточка" },
                { 7, "Перечень" },
                { 31, "Месяц" },
                { 63, "Год" },
            };

            var typeDocDict = new Dictionary<int, string> {
                { 1, "Накладная" },
                { 2, "Ведомость" },
                { 3, "Акт" },
                { 4, "Карточка" }
            };

            //anonymous methods in a generalized and simple way
            Func<IEnumerable<int>> getTopTwo = () => hierarchyDict
                            .OrderByDescending(x => x.Key)
                            .Where(x => x.Key <= typeDoc)
                            .Select(x => x.Key)
                            .Take(2);

            var sqlIn = string.Join(", ", getTopTwo.Invoke());

            IList<TreeNode> tree = new List<TreeNode>();

            #region Query
            /* 04.08.2017
            * It query converts flatted table to hierarchy table (The hierarchy deep is defined by group predicate)
            *
            * [Id] - primary key
            * [parentId] - parents element Id
            * [groupId] - group Id
            * [rankInGr] - element primary key in group
            * [treeLevel] - height of tree
            * [levelName] - custom group tree node name
            * [searchkey] - key for search in plane (source table)
            * [label] - description for rendering purpose
            * [rootKey] - hierarchyid base on hashbyte
            */
            //--Warning weakness!
            //--The order must be same in each aggregation functions
            var query = $@"
            Declare @tree TABLE(
	            [parentId] BIGINT,		[Id] BIGINT,				[groupId] INT,				[rankInGr] INT,
	            [treeLevel] SMALLINT,	[levelName] NVARCHAR(30),   [searchkey] NVARCHAR(30),	[label] NVARCHAR(30),
	            [count] BIGINT,			[rootKey] varbinary(1000) primary key,                  [strKey] NVARCHAR(MAX)
            );

            ;WITH grSubResult AS (
                SELECT
                    [Id] = ROW_NUMBER() OVER(ORDER BY YEAR(DTBUHOTCHET) DESC, MONTH(kn.DTBUHOTCHET) DESC, kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [groupId] = DENSE_RANK() OVER(ORDER BY YEAR(DTBUHOTCHET) DESC, MONTH(kn.DTBUHOTCHET) DESC, kn.KEYKRT DESC),
                    [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY YEAR(DTBUHOTCHET) DESC, MONTH(kn.DTBUHOTCHET) DESC, kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [treeLevel] = GROUPING_ID( YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.tdoc, knos.nomot),
                    --[level_card] = GROUPING(knos.id_kart),
                    [period] =  CONVERT(DATE, N'01.' + CONVERT(NVARCHAR(2),MONTH(kn.DTBUHOTCHET)) + N'.' + CONVERT(NVARCHAR(4),YEAR(kn.DTBUHOTCHET)) ),
                    [year] = YEAR(kn.DTBUHOTCHET),
                    [month] = MONTH(kn.DTBUHOTCHET),
                    [scroll] = kn.NKRT, kn.KEYKRT,
                    [card] = knos.NKRT, knos.id_kart,
		            [typeDoc] = knos.tdoc,
		            [docum] = knos.nomot,
                    [count] = COUNT(*),
                    [rootKey] = HASHBYTES('md5',
			            ISNULL(Convert(nvarchar(4),YEAR(DTBUHOTCHET)),N'') +
			            ISNULL(N'-->' + Convert(nvarchar(2),MONTH(kn.DTBUHOTCHET)),N'') +
			            ISNULL(N'-->' + convert(nvarchar(20), kn.KEYKRT), N'') +
			            ISNULL(N'-->' + convert(nvarchar(10), kn.NKRT), N'') +
			            ISNULL(N'-->' + Convert(nvarchar(10),knos.id_kart),N'') +
			            ISNULL(N'-->' + Convert(nvarchar(10),knos.nkrt),N'') +
			            ISNULL(N'-->' + Convert(nvarchar(1), knos.tdoc), N'') +
			            ISNULL(N'-->' + Convert(nvarchar(10),knos.nomot), N''))
                FROM [dbo].[krt_Naftan_orc_sapod] AS knos INNER JOIN [dbo].[krt_Naftan] AS kn
                    ON kn.KEYKRT = knos.keykrt
                WHERE knos.tdoc > 0 AND knos.Id > 0
                GROUP BY GROUPING SETS(
                       --(),
                        (YEAR(DTBUHOTCHET)),
                        (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET)),
                        (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT),
		                --(kn.KEYKRT, kn.NKRT, knos.id_kart),
                        (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt),
		                (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc),
		                (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc, knos.nomot)
	                )
                )

                Insert into @tree
                    SELECT
                        [parentId] = CASE [treeLevel]
		                WHEN 0 THEN MAX(Id) OVER (PARTITION BY [year], [month], KEYKRT, id_kart, typeDoc)
		                WHEN 1 THEN MAX(Id) OVER (PARTITION BY [year], [month], KEYKRT, id_kart)
		                WHEN 3 THEN MAX(Id) OVER (PARTITION BY [year], [month], KEYKRT)
		                WHEN 7 THEN MAX(Id) OVER (PARTITION BY [year], [month])
		                WHEN 31 THEN MAX(Id) OVER (PARTITION BY [year])
	                ELSE 0 END,
	                [Id], [groupId], [rankInGr], [treeLevel],
	                [levelName] = CASE [treeLevel]
		                WHEN 0 THEN N'Документ'
		                WHEN 1 THEN N'Тип документа'
		                WHEN 3 THEN N'Карточка'
		                WHEN 7 THEN N'Перечень'
		                WHEN 31 THEN N'Месяц'
		                WHEN 63 THEN N'Год'
	                ELSE NULL END,
	                [searchkey] = CASE [treeLevel]
		                WHEN 0 THEN convert(nvarchar(25), [docum])
		                WHEN 1 THEN convert(nvarchar(25), [typeDoc])
		                WHEN 3 THEN convert(nvarchar(25), [id_kart])
		                WHEN 7 THEN convert(nvarchar(25), [keykrt])
		                WHEN 31 THEN convert(nvarchar(25), [month])
		                WHEN 63 THEN convert(nvarchar(25), [year])
	                ELSE NULL END,
	                [label] = CASE [treeLevel]
		                WHEN 0 THEN Convert(nvarchar(25), [docum])
		                WHEN 1 THEN Case [typeDoc] when 1 then N'Накладная' when 2 then N'Ведомость' when 3 then N'Акт' Else N'Карточка' End
		                WHEN 3 THEN [card]
		                WHEN 7 THEN CONVERT(NVARCHAR(10),[scroll])
		                WHEN 31 THEN DATENAME(MONTH,[period])
		                WHEN 63 THEN CONVERT(NVARCHAR(4),[year])
	                ELSE NULL END,
	                [count], [rootKey],
                    [strKey] = N'0x' + cast(N'' as xml).value('xs:hexBinary(sql:column(""rootKey"") )', 'nvarchar(max)')
                FROM grSubResult as gr
                WHERE  [treeLevel] IN ( { sqlIn } )
                ORDER BY [year] DESC, [month], KEYKRT DESC, id_kart desc, gr.[typeDoc] desc, [docum] desc;

            select * from @tree" + (rootKey == null ? "" : $@" where [parentId] = (select Id from @tree where [rootKey] = {rootKey})") +
            " Order by [searchkey] desc";
            #endregion

            try
            {
                IList<TreeNode> result;
                using (this._provider.Uow = new UnitOfWork())
                {
                    result = this._provider.Uow.GetRepository<krt_Naftan_orc_sapod>().SqlQuery<TreeNode>(query).ToList();
                }

                tree = (result.Count > 0 && rootKey == null) ? result.FillRecursive() : result;
            }
            catch (Exception ex)
            {
                this._provider.Log.DebugFormat("Exception: {0}", ex.Message);
            }

            return tree;
        }
    }
}