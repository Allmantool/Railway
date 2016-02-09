using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {

    public class IndexModelView {
        public IEnumerable<krt_Naftan> ListKrtNaftan { get; set; }
        [Required(ErrorMessage = @"Wrong Date?")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMMM yyyy}")]
        public DateTime? ReportPeriod { get; set; }
    }
}