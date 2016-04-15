using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Routing;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    /// <summary>
    /// Controller for work with general filters
    /// </summary>
    public class FilterController : Controller {
        /// <summary>
        /// Update render filter menu on page
        /// </summary>
        /// <param name="values">Values of current filter</param>
        /// <param name="currentFilter">Name current filter</param>
        /// <param name="scopeFilters">Names of all filters on current page</param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Menu(string[] values, string currentFilter, string[] scopeFilters) {
            if (Request.IsAjaxRequest())
                return PartialView("_Menu");
            else
                return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
        }
    }
}