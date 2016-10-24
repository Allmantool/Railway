using System.Collections.Generic;
using NaftanRailway.WebUI.ViewModels;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.BLL.POCO;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewsModels {
    public class DetailModelView {
        public string Title { get; set; }
        public ScrollLineDTO Scroll { get; set; }
        public IEnumerable<ScrollDetailDTO> ListDetails { get; set; }
        public IEnumerable<CheckListFilter> Filters { get; set; }
        public PagingInfo PagesInfo { get; set; }
    }
}