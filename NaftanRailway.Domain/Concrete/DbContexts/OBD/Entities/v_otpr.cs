using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_otpr
    {
        public int id { get; set; }
        public Nullable<int> state { get; set; }
        public Nullable<short> oper { get; set; }
        public Nullable<System.DateTime> date_oper { get; set; }
        public string n_otpr { get; set; }
        public string cod_kl_otpr { get; set; }
        public string cod_klient_pol { get; set; }
        public string g6 { get; set; }
        public string g4 { get; set; }
        public string g16 { get; set; }
        public string g8 { get; set; }
        public string g11 { get; set; }
        public string cod_tvk_etsng { get; set; }
        public string cod_tvk_algng { get; set; }
        public Nullable<int> massa_otpr { get; set; }
        public string name_plat { get; set; }
        public Nullable<int> type_doc { get; set; }
        public string adr_otpr { get; set; }
        public string adr_pol { get; set; }
        public string nam_otpr { get; set; }
        public string nam_pol { get; set; }
        public string fio_tk { get; set; }
        public Nullable<long> eid { get; set; }
        public Nullable<long> eid_zag { get; set; }
    }
}
