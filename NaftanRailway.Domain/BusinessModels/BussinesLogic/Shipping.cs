
using System.Collections.Generic;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// Return general information about 3 entity (v_otpr,v_o_v,entsg)
    /// </summary>
    public class Shipping {
        private bool IsSelected { get; set; }
        public v_otpr VOtpr { get; set; }
        public IEnumerable<v_o_v> Vov { get; set; }
        public IEnumerable<etsng> Etsng { get; set; }

        public Shipping() {
            IsSelected = true;
        }

        public override string ToString() {
            return VOtpr.nam_otpr;
        }
    }
}
