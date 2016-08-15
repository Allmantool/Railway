using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    //[ModelBinder(typeof(ChangeDateModalBinding))]
    public class IndexModelView {
        /// <summary>
        /// List Scroll rows
        /// </summary>
        public IEnumerable<krt_Naftan> ListKrtNaftan { get; set; }
        /// <summary>
        /// Primary key for selecting scroll (nkrt & reportPeriod)
        /// </summary>
        [Required]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MMMM yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportPeriod { get; set; }
        public int Nkrt { get; set; }
        /// <summary>
        /// Info about pagination (count page, current page, line on the page and etc)
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// For change date method
        /// </summary>
        public bool MultiDate { get; set; }
    }
}