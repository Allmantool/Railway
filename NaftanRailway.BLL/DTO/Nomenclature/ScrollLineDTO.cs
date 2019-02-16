using System;
namespace NaftanRailway.BLL.DTO.Nomenclature {
    public class ScrollLineDTO {
        public long KEYKRT { get; set; }
        public int NKRT { get; set; }
        public int NTREB { get; set; }
        public System.DateTime DTBUHOTCHET { get; set; }
        public DateTime? DTTREB { get; set; }
        public DateTime? DTOPEN { get; set; }
        public DateTime? DTCLOSE { get; set; }
        public decimal SMTREB { get; set; }
        public decimal NDSTREB { get; set; }
        public short U_KOD { get; set; }
        public string P_TYPE { get; set; }
        public System.DateTime DATE_OBRABOT { get; set; }
        public bool IN_REAL { get; set; }
        public int RecordCount { get; set; }
        public System.DateTime StartDate_PER { get; set; }
        public System.DateTime EndDate_PER { get; set; }
        public bool SignAdjustment_list { get; set; }
        public string Scroll_Sbor { get; set; }
        public bool Confirmed { get; set; }
        public byte? ErrorState { get; set; }
        public string ErrorMsg { get; set; }
        public byte CounterVersion { get; set; }
    }
}
