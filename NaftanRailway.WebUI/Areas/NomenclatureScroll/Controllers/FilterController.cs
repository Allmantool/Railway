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
        [ChildActionOnly]
        public ActionResult Menu(int numberScroll, int reportYear,IList<CheckListFilterModel> filters) {
            var findKrt = _bussinesEngage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).FirstOrDefault();
            
            if (Request.IsAjaxRequest() && findKrt != null) {
                var updateFilter = new[]{
                    new CheckListFilterModel(){
                        AllAvailableValues = filters.First(x => x.SortFieldName == "nkrt").AllAvailableValues,
                        CheckedValues = _bussinesEngage.GetGroup<krt_Naftan_orc_sapod, string>(
                            x => x.nkrt,
                            x => x.keykrt == findKrt.KEYKRT,
                            x => x.nkrt),
                        SortFieldName = "nkrt"
                    },
                    new CheckListFilterModel(){
                        AllAvailableValues = filters.First(x => x.SortFieldName == "tdoc").AllAvailableValues,
                        CheckedValues = _bussinesEngage.GetGroup<krt_Naftan_orc_sapod, string>(
                            x => x.tdoc.ToString(), 
                            x => x.keykrt == findKrt.KEYKRT, 
                            x => x.tdoc.ToString()),
                        SortFieldName = "tdoc"
                    },
                    new CheckListFilterModel(){
                        AllAvailableValues = filters.First(x => x.SortFieldName == "vidsbr").AllAvailableValues,
                        CheckedValues = _bussinesEngage.GetGroup<krt_Naftan_orc_sapod, string>(
                            x => x.vidsbr.ToString(), 
                            x => x.keykrt == findKrt.KEYKRT, 
                            x => x.vidsbr.ToString()),
                        SortFieldName = "vidsbr"
                    }
            };
                return Json(updateFilter, JsonRequestBehavior.DenyGet);
                //return PartialView("_FilterMenu", filters);
            }
            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
        }
    }
}