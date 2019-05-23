using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_pam_vag
    {
        public int id_vag { get; set; }
        public int id_ved { get; set; }
        public string nomvag { get; set; }
        public string kodgr { get; set; }
        public Nullable<short> kodop { get; set; }
        public Nullable<System.DateTime> d_pod { get; set; }
        public Nullable<System.DateTime> d_ub { get; set; }
    }
}
