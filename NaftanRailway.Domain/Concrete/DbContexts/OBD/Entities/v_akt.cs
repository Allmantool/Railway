using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_akt
    {
        public int id { get; set; }
        public string kodls { get; set; }
        public Nullable<System.DateTime> dakt { get; set; }
        public string kodkl { get; set; }
        public string nkrt { get; set; }
        public Nullable<int> id_kart { get; set; }
        public Nullable<int> state { get; set; }
        public string nakt { get; set; }
    }
}
