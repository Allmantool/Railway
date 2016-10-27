namespace NaftanRailway.WebUI.ViewModels {
    public class InfoLineViewModel {
        /// <summary>
        /// object what responde for storage info about one line in session storage
        /// </summary>
        public ShippingInfoLine DocumentPackLine { get; set; }
        /// <summary>
        /// Need for return previos page with saving state
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}