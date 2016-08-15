using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using LinqKit;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.Domain.ExpressionTreeExtensions;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    /// <summary>
    /// Controller for work with general filters
    /// </summary>
    public class FilterController : Controller {
        private readonly IBussinesEngage _bussinesEngage;
        public FilterController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }
        /// <summary>
        /// Update render filter menu on page
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="filters">Set of filters</param>
        /// <param name="numberScroll"></param>
        /// <returns></returns>
        [HttpPost]
        //[ChildActionOnly]
        public ActionResult Menu(int numberScroll, int reportYear, IList<CheckListFilterModel> filters) {
            var findKrt = _bussinesEngage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).FirstOrDefault();

            //build lambda expression basic on active filter(linqKit)
            var finalPredicate = PredicateBuilder.True<krt_Naftan_orc_sapod>().And(x => x.keykrt == findKrt.KEYKRT);
            finalPredicate = filters.Where(x => x.ActiveFilter)
                .Aggregate(finalPredicate, (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()));

            //update filters
            if (Request.IsAjaxRequest() && findKrt != null) {
                foreach (var item in filters) {
                    item.CheckedValues = _bussinesEngage.GetGroup(
                        PredicateExtensions.GroupPredicate<krt_Naftan_orc_sapod>(item.SortFieldName).Expand(),
                        finalPredicate.Expand()
                    );
                }

                //return Json(filters, JsonRequestBehavior.DenyGet);
                return PartialView("_FilterMenu", filters);
            }
            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
        }
    }
}