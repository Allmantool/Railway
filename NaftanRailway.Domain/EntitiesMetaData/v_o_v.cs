using System;
using System.Collections.Generic;

namespace NaftanRailway.Domain.Concrete.DbContext.OBD {
    public partial class v_o_v {
        public bool IsSelected { get; set; }
        public ICollection<v_pam_vag> Pam_vag { get; set; }
        public ICollection<v_akt_vag> Akt_vag { get; set; }
        public v_o_v() {
            IsSelected = true;
        }
        public override string ToString() {
            return n_vag;
        }
    }
}
