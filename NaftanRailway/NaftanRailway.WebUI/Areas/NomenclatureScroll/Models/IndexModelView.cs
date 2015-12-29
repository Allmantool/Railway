using System.Collections.Generic;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    public class IndexModelView {
        public IEnumerable<krt_Naftan> ListKrtNaftan { get; set; }
        public string FilterScroll { get; set; }
    }
}