using System;
using System.Collections.Generic;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.Abstract {
    public interface INomenclatureModule : IDisposable {
        IBussinesEngage _engage { get; set; }
        IEnumerable<krt_Naftan> SkipScrollTable(int page, int initialSizeItem, out int recordCount);
        bool AddKrtNaftan(int numberScroll, int reportYear, out string msgError);
        /// <summary>
        /// Change Reporting date
        /// </summary>
        /// <param name="period"></param>
        /// <param name="numberScroll"></param>
        /// <param name="multiChange"></param>
        /// <returns></returns>
        IEnumerable<krt_Naftan> ChangeBuhDate(DateTime period, int numberScroll, bool multiChange = true);
        bool EditKrtNaftanOrcSapod(long keykrt, long keysbor, decimal nds, decimal summa);
    }
}
