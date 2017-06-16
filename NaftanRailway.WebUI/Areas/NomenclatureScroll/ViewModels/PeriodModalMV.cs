using NaftanRailway.BLL.DTO.Nomenclature;
using System;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewModels {
    public class PeriodModalMV {
        public ScrollLineDTO Item { get; set; }
        public DateTime Period { get; set; }
        public bool Multimode { get; set; }
    }
}