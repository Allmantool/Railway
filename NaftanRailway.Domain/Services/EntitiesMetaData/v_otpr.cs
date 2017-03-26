using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NaftanRailway.Domain.Concrete.DbContexts.OBD {
    [MetadataType(typeof(ShippingMetaData))]
    public partial class v_otpr {
    }

    internal class ShippingMetaData {
        //[HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [Display(Name = "Номер отправки")]
        [Required(ErrorMessage = "Пожайлуйста введите номер отправки")]
        [StringLength(8, MinimumLength = 8)]
        public string n_otpr { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? date_oper { get; set; }

        [DisplayName("Адрес")]
        public string g8 { get; set; }

        [DisplayName("Груз")]
        [DataType(DataType.MultilineText)]
        public string g11 { get; set; }

        [DisplayName("Исполнитель")]
        public string fio_tk { get; set; }

        [NotMapped]
        public string cim_adr_otpr { get; set; }
        [NotMapped]
        public string cim_nam_otpr { get; set; }
        [NotMapped]
        public string cim_adr_pol { get; set; }
        [NotMapped]
        public string cim_num_dog_exporter { get; set; }
        [NotMapped]
        public string cim_app_doc { get; set; }
        [NotMapped]
        public string cim_inter_code_station { get; set; }
        [NotMapped]
        public string cim_code_country { get; set; }
        [NotMapped]
        public string cim_place_reception { get; set; }
        [NotMapped]
        public string cim_is_app_smgs { get; set; }
        [NotMapped]
        public string cim_carrierStatement { get; set; }
        [NotMapped]
        public string cim_place_delivery { get; set; }
        [NotMapped]
        public string cim_other_carriers { get; set; }
    }
}