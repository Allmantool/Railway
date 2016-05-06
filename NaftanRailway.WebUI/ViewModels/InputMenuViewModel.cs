using System;
using System.Web.Mvc;
using NaftanRailway.WebUI.Infrastructure.Binders;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// User Input object
    /// </summary>
    //[ModelBinder(typeof(InputMenuModelBinder ))]
    public class InputMenuViewModel {
        /// <summary>
        /// Reporting period (Отчётный период месяц/год)
        /// </summary>
        public DateTime? ReportPeriod { get; set; }
        /// <summary>
        /// template for shipping searched
        /// </summary>
        public string ShippingChoise {get;set;}
    }
}