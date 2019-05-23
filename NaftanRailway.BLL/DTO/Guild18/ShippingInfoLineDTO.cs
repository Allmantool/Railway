using System.Collections.Generic;
using Railway.Domain.Concrete.DbContexts.Mesplan.Entities;
using Railway.Domain.Concrete.DbContexts.OBD.Entities;

namespace NaftanRailway.BLL.DTO.Guild18 {
    /// <summary>
    /// PreVeiw informataion about one invoice /dispatch operation
    /// </summary>
    public class ShippingInfoLineDTO {
        public bool IsSelected { get; set; }

        public v_otpr Shipping { get; set; }

        public IList<v_o_v> WagonsNumbers { get; set; }

        public Etsng CargoEtsngName { get; set; }

        public int Warehouse { get; set; }

        public ShippingInfoLineDTO() {
            this.IsSelected = true;
        }
    }
}