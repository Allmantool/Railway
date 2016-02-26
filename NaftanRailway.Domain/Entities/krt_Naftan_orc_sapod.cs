using System;
using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.Domain.Concrete.DbContext.ORC {
    [MetadataType(typeof(MetaDataKrt_Naftan_orc_sapod))]
    public partial class krt_Naftan_orc_sapod { }

    internal class MetaDataKrt_Naftan_orc_sapod {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#,#}")]
        //[DataType(DataType.Currency)]
        public Nullable<decimal> nds { get; set; }
    }
}
