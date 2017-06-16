using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// PreVeiw informataion about one invoice /dispatch operation
    /// </summary>
    public class ShippingInfoLine {
        public bool IsSelected { get; set; }

        //[DisplayName("Номер накладной")]
        //public v_otpr Shipping { get; set; }

        //[DisplayName("Номера вагона(ов)")]
        //[UIHint("v_o_v")]
        //public IList<v_o_v> WagonsNumbers { get; set; }

        //[DisplayName("Наименование груза (ETСНГ)")]
        //public etsng CargoEtsngName { get; set; }

        [DisplayName("Номер склада")]
        [Range(1, 1000, ErrorMessage = "Пожалуйста введите верный номер склада!")]
        [Required(ErrorMessage = "Пожалуйста введите номер склада!")]
        public int Warehouse { get; set; }

        public ShippingInfoLine() {
            IsSelected = true;
        }
    }
}