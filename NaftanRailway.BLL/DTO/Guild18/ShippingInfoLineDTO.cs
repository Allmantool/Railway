using System.Collections.Generic;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;

namespace NaftanRailway.BLL.DTO.Guild18 {
    /// <summary>
    /// PreVeiw informataion about one invoice /dispatch operation
    /// </summary>
    public class ShippingInfoLineDTO {
        public bool IsSelected { get; set; }

        public v_otpr Shipping { get; set; }

        public IList<v_o_v> WagonsNumbers { get; set; }

        public etsng CargoEtsngName { get; set; }

        public int Warehouse { get; set; }

        public ShippingInfoLineDTO() {
            IsSelected = true;
        }
    }
}