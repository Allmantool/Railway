//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NaftanRailway.Domain.Concrete.DbContext.OBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class v_akt_sb
    {
        public int id { get; set; }
        public int id_akt { get; set; }
        public int id_vag { get; set; }
        public string kod_sb { get; set; }
        public Nullable<decimal> sum_sb { get; set; }
        public Nullable<decimal> nds_sb { get; set; }
        public string textm { get; set; }
    }
}