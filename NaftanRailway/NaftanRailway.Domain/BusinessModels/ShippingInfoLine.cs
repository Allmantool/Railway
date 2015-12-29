using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// One info row(line)
    /// </summary>
    public class ShippingInfoLine {
        [DisplayName("Склад")]
        [Range(1,1000,ErrorMessage = "Пожалуйста введите верный номер склада!")]
        [Required(ErrorMessage = "Пожалуйста введите номер склада!")]
        public int Warehouse { get; set; }

        [DisplayName("Отправки")]
        public v_otpr Shipping { get; set; }

        [DisplayName("Номера вагона(ов)")]
        [UIHint("v_o_v")]
        public IList<v_o_v> WagonsNumbers { get; set; }

        [DisplayName("Номера ведомости(ей)")]
        [UIHint("Bill")]
        public IList<Bill> Bills { get; set; }

        [DisplayName("Номера акта(ов)")]
        [UIHint("Certificate")]
        public IList<Certificate> Acts { get; set; }

        [DisplayName("Номера карточки(ей)")]
        [UIHint("AccumulativeCard")]
        public IList<AccumulativeCard> Cards { get; set; }

        [DisplayName("Багаж")]
        [UIHint("Luggage")]
        public IList<Luggage> Luggages { get; set; }
    }
}
