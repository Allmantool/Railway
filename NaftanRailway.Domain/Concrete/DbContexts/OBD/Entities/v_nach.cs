﻿using System;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_nach
    {
        public int id { get; set; }
        public int id_kart { get; set; }
        public Nullable<int> id_otpr { get; set; }
        public string cod_kl { get; set; }
        public Nullable<int> type_doc { get; set; }
        public Nullable<short> oper { get; set; }
        public string cod_sbor { get; set; }
        public Nullable<System.DateTime> date_raskr { get; set; }
        public string num_doc { get; set; }
        public Nullable<decimal> cena { get; set; }
        public Nullable<decimal> kol { get; set; }
        public Nullable<decimal> summa { get; set; }
        public Nullable<decimal> nds { get; set; }
        public string textm { get; set; }
        public Nullable<int> id_ed { get; set; }

        public virtual v_kart v_kart { get; set; }
    }
}
