using System;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace NaftanRailway.WebUI.HtmlHelpers {
    /// <summary>
    /// Partial views don't support @section by default
    /// </summary>
    public static class HtmlExtensions {

        /// <summary>
        /// Give unical Guid for script
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template) {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }
        
        /// <summary>
        /// Searched and then Write directly into html (unical script)
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper) {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys) {
                if (key.ToString().StartsWith("_script_")) {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null) {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }
    }
}