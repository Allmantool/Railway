//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Railway.Domain.Concrete.DbContexts.ORC
{
    using System;
    using System.Collections.Generic;
    
    public partial class krt_Naftan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public krt_Naftan()
        {
            this.krt_Naftan_orc_sapod = new HashSet<krt_Naftan_orc_sapod>();
        }
    
        public long KEYKRT { get; set; }
        public int NKRT { get; set; }
        public int NTREB { get; set; }
        public System.DateTime DTBUHOTCHET { get; set; }
        public Nullable<System.DateTime> DTTREB { get; set; }
        public Nullable<System.DateTime> DTOPEN { get; set; }
        public Nullable<System.DateTime> DTCLOSE { get; set; }
        public decimal SMTREB { get; set; }
        public decimal NDSTREB { get; set; }
        public short U_KOD { get; set; }
        public string P_TYPE { get; set; }
        public System.DateTime DATE_OBRABOT { get; set; }
        public bool IN_REAL { get; set; }
        public int RecordCount { get; set; }
        public System.DateTime StartDate_PER { get; set; }
        public System.DateTime EndDate_PER { get; set; }
        public bool SignAdjustment_list { get; set; }
        public string Scroll_Sbor { get; set; }
        public bool Confirmed { get; set; }
        public Nullable<byte> ErrorState { get; set; }
        public string ErrorMsg { get; set; }
        public byte CounterVersion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<krt_Naftan_orc_sapod> krt_Naftan_orc_sapod { get; set; }
    }
}
