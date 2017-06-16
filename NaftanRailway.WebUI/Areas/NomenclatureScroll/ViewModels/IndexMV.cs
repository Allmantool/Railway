using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewModels {
    //[ModelBinder(typeof(ChangeDateModalBinding))]
    public class IndexMV {
        /// <summary>
        /// List Scroll rows
        /// </summary>
        public IEnumerable<ScrollLineDTO> ListKrtNaftan { get; set; }

        /// <summary>
        /// Primary key for selecting scroll (nkrt & reportPeriod)
        /// </summary>
        [Required]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportPeriod { get; set; }

        /// <summary>
        /// Info about pagination (count page, current page, line on the page and etc)
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        public IEnumerable<DateTime> RangePeriod { get; set; }

        /// <summary>
        /// For change date method
        /// </summary>
        public bool MultiDate { get; set; }

        public int Nkrt { get; set; }
    }
}