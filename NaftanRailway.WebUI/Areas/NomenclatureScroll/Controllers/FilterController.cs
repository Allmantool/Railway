using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using LinqKit;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
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
        public ActionResult Menu(int numberScroll, int reportYear,IList<CheckListFilterModel> filters) {
            var findKrt = _bussinesEngage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).FirstOrDefault();

            //upply filters(linqKit)
            var finalPredicate = PredicateBuilder.True<krt_Naftan_orc_sapod>().And(x => x.keykrt == findKrt.KEYKRT);
            finalPredicate = filters.Aggregate(finalPredicate, (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()));

            if (Request.IsAjaxRequest() && findKrt != null) {
                var updateFilter = new[]{
                    new CheckListFilterModel(){
                        AllAvailableValues = filters.First(x => x.SortFieldName == "nkrt").AllAvailableValues,
                        CheckedValues = _bussinesEngage.GetGroup(
                            x => x.nkrt,
                            finalPredicate.Expand(),
                            x => x.nkrt),
                        SortFieldName = "nkrt"
                    },
                    new CheckListFilterModel(){
                        AllAvailableValues = filters.First(x => x.SortFieldName == "tdoc").AllAvailableValues,
                        CheckedValues = _bussinesEngage.GetGroup(
                            x => x.tdoc.ToString(), 
                            finalPredicate.Expand(), 
                            x => x.tdoc.ToString()),
                        SortFieldName = "tdoc"
                    },
                    new CheckListFilterModel(){
                        AllAvailableValues = filters.First(x => x.SortFieldName == "vidsbr").AllAvailableValues,
                        CheckedValues = _bussinesEngage.GetGroup(
                            x => x.vidsbr.ToString(), 
                            finalPredicate.Expand(), 
                            x => x.vidsbr.ToString()),
                        SortFieldName = "vidsbr"
                    }
            };
                //return Json(updateFilter, JsonRequestBehavior.DenyGet);
                return PartialView("_FilterMenu", filters);
            }
            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
        }
    }
}