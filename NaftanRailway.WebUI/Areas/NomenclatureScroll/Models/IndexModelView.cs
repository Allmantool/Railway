using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {

    public class IndexModelView {
        public IEnumerable<krt_Naftan> ListKrtNaftan { get; set; }
        [Required]
        [DataType(DataType.Date),DisplayFormat(DataFormatString = "{0:MMMM yyyy}",ApplyFormatInEditMode = true)]
        public DateTime? ReportPeriod { get; set; }
    }
}