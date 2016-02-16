using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {

    public class IndexModelView {
        /// <summary>
        /// List Scroll rows
        /// </summary>
        public IEnumerable<krt_Naftan> ListKrtNaftan { get; set; }
        [Required]
        [DataType(DataType.Date),DisplayFormat(DataFormatString = "{0:MMMM yyyy}",ApplyFormatInEditMode = true)]
        public DateTime? ReportPeriod { get; set; }
        /// <summary>
        /// Info about pagination (count page, current page, line on the page and etc)
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
    }
}