using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    /// <summary>
    /// Controller for work with general filters
    /// </summary>
    public class FilterController : Controller {
        /// <summary>
        /// Update render filter menu on page
        /// </summary>
        /// <param name="filters">Set of filters</param>
        /// <returns></returns>
        //[ChildActionOnly]
        public ActionResult Menu(IEnumerable<CheckListFilterModel> filters) {
            if (Request.IsAjaxRequest())
                return PartialView("_FilterMenu", filters);

            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
        }
    }
}