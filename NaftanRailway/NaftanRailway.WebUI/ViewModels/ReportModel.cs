using System;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// Report view model
    /// </summary>
    public class ReportModel {
        public string n_otpr { get; set; }
        public string n_vag { get; set; }
        public DateTime date_oper { get; set; }
        public string cod_sb  { get; set; }
        public string nameSb { get; set; }
        public decimal sum_no_nds { get; set; }
        public decimal nds { get; set; }
        public decimal sum_nds { get; set; }
        public decimal sum_with_nds { get; set; }
        public string n_per_list{ get; set; }
        public string n_kart { get; set; }
        public string note { get; set; }
        public string warehouse { get; set; }
    }
}
