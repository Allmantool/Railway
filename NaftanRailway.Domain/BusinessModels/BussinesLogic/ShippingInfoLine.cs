using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// PreVeiw informataion about one delivery
    /// </summary>
    public class ShippingInfoLine {
        [DisplayName("Отправки")]
        //[Range(1, 1000, ErrorMessage = "Пожалуйста введите верный номер склада!")]
        //[Required(ErrorMessage = "Пожалуйста введите номер склада!")]
        public v_otpr Shipping { get; set; }

        [DisplayName("Номера вагона(ов)")]
        [UIHint("v_o_v")]
        public IEnumerable<v_o_v> WagonsNumbers { get; set; }

        [DisplayName("Наименование груза (ETСНГ)")]
        public etsng CargoEtsngName { get; set; }

        [DisplayName("Номер склада")]
        public int Warehouse { get; set; }
    }
}