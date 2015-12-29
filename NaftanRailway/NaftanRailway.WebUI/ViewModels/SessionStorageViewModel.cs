using NaftanRailway.Domain.BusinessModels;

namespace NaftanRailway.WebUI.ViewModels {
    public class SessionStorageViewModel {
        public SessionStorage Storage { get; set; }
        /// <summary>
        /// Need for return previos page with saving state
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}