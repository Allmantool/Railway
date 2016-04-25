using System.Collections.Generic;
using System.Linq;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    /// <summary>
    /// General model for drop down checkbox list (per one field)
    /// </summary>
    public class CheckListFilterModel {
        public IEnumerable<string> AllAvailableValues { get; set; }
        public IEnumerable<string> CheckedValues { get; set; }
        public string SortFieldName { get; set; }
        /// <summary>
        /// By default check all values on list box
        /// </summary>
        public CheckListFilterModel(IEnumerable<string> allAvailableValues) {
            var availableValues = allAvailableValues as string[] ?? allAvailableValues.ToArray();

            AllAvailableValues = availableValues;
            CheckedValues = availableValues;
        }
        public CheckListFilterModel() { }
    }
}