using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NaftanRailway.Domain.Concrete.DbContext.ORC {
    [MetadataType(typeof(MetaDataKrt_Naftan_orc_sapod))]
    public partial class krt_Naftan_orc_sapod { }

    /*Buddy classes must be defined in the same namespace and must also be partial classes.(only in Freeman book)*/
    [DisplayName("Details Krt_Naftan")]
    internal  sealed class MetaDataKrt_Naftan_orc_sapod {
        [HiddenInput(DisplayValue = false)]
        /* the ScaffoldColumn attribute doesn’t have an effect on the per-property helpers, such as EditorFor.*/
        [ScaffoldColumn(false)]
        public long keykrt { get; set; }
        [HiddenInput(DisplayValue = false)]
        [ScaffoldColumn(false)]
        public long keysbor { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#,#}")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> nds { get; set; }
        [Display(Name = "Сумма с НДС:")]
        [DataType(DataType.Currency)]
        public decimal sm { get; set; }
        [Display(Name = "Сумма:")]
        [DataType(DataType.Currency)]
        public decimal sm_no_nds { get; set; }
        [Display(Name = "НДС:")]
        [DataType(DataType.Currency)]
        /*Using Metadata to Select a Display Template (have build-in and custom template)
        [UIHint("Number")]*/
        public decimal sm_nds { get; set; }
    }
}
