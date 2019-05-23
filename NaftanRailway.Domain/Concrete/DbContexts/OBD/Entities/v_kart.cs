using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_kart
    {
        public int id { get; set; }
        public string num_kart { get; set; }
        public Nullable<System.DateTime> date_okrt { get; set; }
        public string cod_pl { get; set; }
        public Nullable<short> type_kart { get; set; }
        public string cod_ls { get; set; }
        public Nullable<decimal> summa { get; set; }
        public Nullable<long> state { get; set; }
        public Nullable<System.DateTime> date_fdu93 { get; set; }
        public Nullable<System.DateTime> date_zkrt { get; set; }
    }
}
