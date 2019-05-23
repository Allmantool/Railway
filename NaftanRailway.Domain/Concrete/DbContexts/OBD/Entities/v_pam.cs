using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_pam
    {
        public int id_ved { get; set; }
        public string nved { get; set; }
        public Nullable<System.DateTime> dved { get; set; }
        public Nullable<System.DateTime> dzakr { get; set; }
        public string kodkl { get; set; }
        public string nkrt { get; set; }
        public Nullable<int> id_kart { get; set; }
        public Nullable<int> state { get; set; }
    }
}
