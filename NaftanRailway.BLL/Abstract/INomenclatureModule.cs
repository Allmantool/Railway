﻿using System;
using System.Collections.Generic;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;
using System.Linq.Expressions;
using NaftanRailway.BLL.DTO.General;
using System.Threading.Tasks;

namespace NaftanRailway.BLL.Abstract {
    public interface INomenclatureModule : IDisposable {
        /// <summary>
        /// Get part of table
        /// </summary>
        /// <typeparam name="T">Type of table</typeparam>
        /// <param name="page"></param>
        /// <param name="initialSizeItem">item per page</param>
        /// <param name="recordCount"> return whole amount of rows</param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> SkipTable<T>(int page, int initialSizeItem, out long recordCount, Expression<Func<T, bool>> predicate);

        IEnumerable<CheckListFilter> InitNomenclatureDetailMenu(long key);
        IEnumerable<CheckListFilter> InitGlobalSearchFilters();

        IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(long key, IList<CheckListFilter> filters, int page,
                                                                   int initialSizeItem, out long recordCount, bool viewWrong = false);
        IList<TreeNode> GetTreeStructure(int typeDoc = 63, string rootKey = null);

        /// <summary>
        /// Update linked filters base on src entity (table)
        /// </summary>
        /// <param name="scroll"></param>
        /// <param name="filters"></param>
        /// <param name="typeFilter">source table type</param>
        /// <returns></returns>
        bool UpdateRelatingFilters(ScrollLineDTO scroll, ref IList<CheckListFilter> filters, EnumTypeFilterMenu typeFilter);

        Task<Tuple<byte[], string>> GetNomenclatureReports(BrowserInfoDTO brInfo, int numberScroll, int reportYear, string serverName,
                                      string folderName, string reportName,
                                      string defaultParameters = @"rs:Format=Excel");

        ScrollLineDTO GetNomenclatureByNumber(int numberScroll, int reportYear);

        IEnumerable<DateTime> GetListPeriod();

        IEnumerable<ScrollLineDTO> AddKrtNaftan(int numberScroll, int reportYear, out string msgError);

        ScrollLineDTO DeleteNomenclature(int numberScroll, int reportYear);

        Task<int> SyncWithOrc();

        /// <summary>
        /// Change Reporting date
        /// </summary>
        /// <param name="period"></param>
        /// <param name="keyScroll"></param>
        /// <param name="multiChange"></param>
        /// <returns></returns>
        IEnumerable<ScrollLineDTO> ChangeBuhDate(DateTime period, long keyScroll, bool multiChange = true);

        bool EditKrtNaftanOrcSapod(ScrollDetailDTO charge);

        ScrollDetailDTO OperationOnScrollDetail(long key, EnumMenuOperation operation);
    }
}