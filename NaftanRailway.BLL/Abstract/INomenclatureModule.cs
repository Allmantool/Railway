using System;
using System.Collections.Generic;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;
using System.Web.Mvc;

namespace NaftanRailway.BLL.Abstract {
    public interface INomenclatureModule : IDisposable {
        IBussinesEngage Engage { get; }

        /// <summary>
        /// Get part of table
        /// </summary>
        /// <typeparam name="T">Type of table</typeparam>
        /// <param name="mode">Type of table throughtout enum</param>
        /// <param name="page"></param>
        /// <param name="initialSizeItem">item per page</param>
        /// <param name="recordCount"> return whole amount of rows</param>
        /// <returns></returns>
        IEnumerable<T> SkipTable<T>(int page, int initialSizeItem, out long recordCount);
        IEnumerable<T> SkipTable<T>(long key, int page, int initialSizeItem);

        IEnumerable<CheckListFilter> InitNomenclatureDetailMenu(long key);
        IEnumerable<ScrollDetailDTO> ApplyNomenclatureDetailFilter(IList<CheckListFilter> filters, int page, byte initialSizeItem, out long recordCount);

        bool UpdateRelatingFilters();

        byte[] GetNomenclatureReports(Controller contr, int numberScroll, int reportYear, string serverName, string folderName, string reportName, string defaultParameters = @"rs:Format=Excel");

        ScrollLineDTO GetNomenclatureByNumber(int numberScroll, int reportYear);

        IEnumerable<ScrollLineDTO> AddKrtNaftan(int numberScroll, int reportYear, out string msgError);

        void SyncWithOrc();

        /// <summary>
        /// Change Reporting date
        /// </summary>
        /// <param name="period"></param>
        /// <param name="numberScroll"></param>
        /// <param name="multiChange"></param>
        /// <returns></returns>
        IEnumerable<ScrollLineDTO> ChangeBuhDate(DateTime period, int numberScroll, bool multiChange = true);

        bool EditKrtNaftanOrcSapod(long keykrt, long keysbor, decimal nds, decimal summa);

        ScrollDetailDTO OperationOnScrollDetail(long key, EnumMenuOperation operation);
    }
}