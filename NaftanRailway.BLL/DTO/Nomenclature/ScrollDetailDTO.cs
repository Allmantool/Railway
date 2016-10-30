﻿using System;

namespace NaftanRailway.BLL.DTO.Nomenclature {
    public class ScrollDetailDTO {
        public long keykrt { get; set; }
        public long keysbor { get; set; }
        public string nomot { get; set; }
        public System.DateTime dt { get; set; }
        public string gruname { get; set; }
        public short vidsbr { get; set; }
        public string namesbr { get; set; }
        public decimal sm_no_nds { get; set; }
        public decimal sm_nds { get; set; }
        public decimal sm { get; set; }
        public decimal stnds { get; set; }
        public string txt { get; set; }
        public string UNI_OTPR { get; set; }
        public string nkrt { get; set; }
        public byte tdoc { get; set; }
        public int ORC_ID_ED { get; set; }
        public Nullable<int> id { get; set; }
        public Nullable<int> id_kart { get; set; }
        public Nullable<int> id_otpr { get; set; }
        public Nullable<System.DateTime> date_raskr { get; set; }
        public string num_doc { get; set; }
        public Nullable<decimal> cena { get; set; }
        public Nullable<decimal> kol { get; set; }
        public Nullable<decimal> summa { get; set; }
        public Nullable<decimal> nds { get; set; }
        public string textm { get; set; }
        public Nullable<int> ID_ED { get; set; }
        public Nullable<byte> ErrorState { get; set; }
    }
}
