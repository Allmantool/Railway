using System.Web.Mvc;

namespace NaftanRailway.WebUI.HtmlHelpers {
    /// <summary>
    /// Html helper => hide rendering logic for scroll
    /// </summary>
    public static class NomenclatureHelpers {
        /// <summary>
        /// Render sum before and after denomination
        /// </summary>
        /// <param name="html"></param>
        /// <param name="markerValue"></param>
        /// <param name="denominationVal"></param>
        /// <returns></returns>
        public static MvcHtmlString DenominationScroll(this HtmlHelper html, long markerValue, decimal denominationVal) {
            var result = (markerValue >= 15072000209229) ? denominationVal.ToString("#,0.00") : denominationVal.ToString("#,#");

            return MvcHtmlString.Create(result);
        }
    }
}