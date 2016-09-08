using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NaftanRailway.Domain.Concrete.DbContexts.OBD {
    [MetadataType(typeof(ShippingMetaData))]
    public partial class v_otpr {}

    internal class ShippingMetaData {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [Display(Name = "Номер отправки")]
        [Required(ErrorMessage = "Пожайлуйста введите номер отправки")]
        [StringLength(8,MinimumLength = 8)]
        public string n_otpr { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        public DateTime? date_oper { get; set; }

        [DisplayName("Адрес")]
        public string g8 { get; set; }

        [DisplayName("Груз")]
        [DataType(DataType.MultilineText)]
        public string g11 { get; set; }

        [DisplayName("Исполнитель")]
        public string fio_tk { get; set; }

        public Nullable<short> oper { get; set; }
    }
}