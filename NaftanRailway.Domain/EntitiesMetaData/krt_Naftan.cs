using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace NaftanRailway.Domain.Concrete.DbContext.ORC {
    [MetadataType(typeof(ObjMetaData))]
    public partial class krt_Naftan {
    }
    internal class ObjMetaData {
        //[ScriptIgnore]
        //public virtual ICollection<krt_Naftan_orc_sapod> krt_Naftan_orc_sapod { get; set; }
    }
}
