
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// Return general information about 3 entity (v_otpr,v_o_v,entsg)
    /// </summary>
    public class Shipping {
        private bool IsSelected { get; set; }
        public v_otpr VOtpr { get; set; }
        public etsng Etsng { get; set; }
        public krt_Guild18 Guild18 { get; set; }
        public Shipping() {
            IsSelected = true;
        }

        public override string ToString() {
            return VOtpr.nam_otpr;
        }
    }
}