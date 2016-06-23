using System;
using System.Collections.Generic;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// User Input object
    /// </summary>
    //[ModelBinder(typeof(InputMenuModelBinder ))]
    public class InputMenuViewModel {
        /// <summary>
        /// Reporting period (Отчётный период месяц/год)
        /// </summary>
        public DateTime ReportPeriod { get; set; }
        /// <summary>
        /// template for shipping searched
        /// </summary>
        public string ShippingChoise { get; set; }
        //Type of dispatch (arrival/sending/all)
        public string SelectedOperCategory { get; set; }
        public IEnumerable<short> TypesOfOperation { get; set; }
    }
}