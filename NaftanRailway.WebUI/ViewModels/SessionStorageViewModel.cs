using NaftanRailway.BLL.Abstract;

namespace NaftanRailway.WebUI.ViewModels {
    public class SessionStorageViewModel {
        public ISessionStorage Storage { get; set; }
        /// <summary>
        /// Need for return previous page with saving state
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}