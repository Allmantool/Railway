//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NaftanRailway.Domain.Concrete.DbContext.OBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class v_pam_vag
    {
        public int id_vag { get; set; }
        public int id_ved { get; set; }
        public string npam { get; set; }
        public string nomvag { get; set; }
        public string sob_s { get; set; }
        public Nullable<short> sob_v { get; set; }
        public string sogl { get; set; }
        public Nullable<short> tipvag { get; set; }
        public string kodgr { get; set; }
        public Nullable<short> kodop { get; set; }
        public Nullable<System.DateTime> d_pod { get; set; }
        public Nullable<System.DateTime> d_ub { get; set; }
    }
}
