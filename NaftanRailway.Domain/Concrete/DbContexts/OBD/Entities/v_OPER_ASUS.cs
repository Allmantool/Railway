using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_OPER_ASUS
    {
        public long id { get; set; }
        public Nullable<long> obj_id { get; set; }
        public Nullable<long> id_vag { get; set; }
        public string cod_oper { get; set; }
        public Nullable<System.DateTime> time_oper { get; set; }
        public string in_vgn { get; set; }
        public Nullable<decimal> ves_gruz { get; set; }
        public Nullable<short> plomb { get; set; }
        public string primech { get; set; }
        public string cod_grpl { get; set; }
        public string cod_gruz { get; set; }
    }
}
