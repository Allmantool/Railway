//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NaftanRailway.Domain.Concrete.DbContext.ORC
{
    using System;
    using System.Collections.Generic;
    
    public partial class orc_sbor
    {
        public long KEYKRT { get; set; }
        public long KEYSBOR { get; set; }
        public string NOMOT { get; set; }
        public Nullable<System.DateTime> DT { get; set; }
        public string GRUNAME { get; set; }
        public Nullable<int> VIDSBR { get; set; }
        public string NAMESBR { get; set; }
        public string TXT { get; set; }
        public Nullable<decimal> SM { get; set; }
        public Nullable<decimal> SM_NDS { get; set; }
        public Nullable<decimal> STNDS { get; set; }
        public string UNI_OTPR { get; set; }
        public string ID_KART { get; set; }
        public string NKRT { get; set; }
        public string P_TYPE { get; set; }
        public Nullable<System.DateTime> DATE_OBRABOT { get; set; }
        public Nullable<int> TDOC { get; set; }
        public Nullable<int> ID_ED { get; set; }
    
        public virtual orc_krt orc_krt { get; set; }
    }
}
