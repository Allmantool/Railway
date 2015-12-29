using System;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// User Input object
    /// </summary>
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