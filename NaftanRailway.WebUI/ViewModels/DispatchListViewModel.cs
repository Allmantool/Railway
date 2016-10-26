using System.Collections.Generic;
using NaftanRailway.BLL.DTO.Guild18;
using NaftanRailway.BLL.Services;

namespace NaftanRailway.WebUI.ViewModels {
    public class DispatchListViewModel {
        /// <summary>
        /// Input menu info
        /// </summary>
        public InputMenuViewModel Menu { get; set; }
        /// <summary>
        /// Collection shipping numbers
        /// </summary>
        public IEnumerable<ShippingDTO> Dispatchs { get; set; }
        /// <summary>
        /// Info about pagination (count page, current page, line on the page and etc)
        /// </summary>
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// Filter for operations
        /// </summary>
        public EnumOperationType OperationCategory { get; set; }
    }
}