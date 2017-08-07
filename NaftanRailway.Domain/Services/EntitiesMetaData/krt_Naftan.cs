using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.Domain.Concrete.DbContexts.ORC
{
    [MetadataType(typeof(ObjMetaData))]
    public partial class krt_Naftan
    {
    }

    internal class ObjMetaData
    {
        [Key]
        public long KEYKRT { get; set; }
        //[ScriptIgnore]
        //public virtual ICollection<krt_Naftan_orc_sapod> krt_Naftan_orc_sapod { get; set; }

        //Optimistic Concurrency
        //[Timestamp()]
        //public byte[] RowVersion { get; set; }
        //[ConcurrencyCheck]
        //public int Version { get; set; }
    }
}