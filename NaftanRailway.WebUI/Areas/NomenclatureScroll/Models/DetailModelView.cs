using System.Collections.Generic;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    public class DetailModelView {
        public krt_Naftan Scroll { get; set; }
        public IEnumerable<krt_Naftan_orc_sapod> CollDetails { get; set; }
        public IEnumerable<CheckListFilterModel> Filters { get; set; }
        public PagingInfo PagesInfo { get; set; }
    }
}