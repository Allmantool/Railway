using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.POCO;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewsModels;
using NaftanRailway.BLL.Services;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    /// <summary>
    /// Controller for work with general filters
    /// </summary>
    public class UserInterfaceController : Controller {
        private readonly IBussinesEngage _bussinesEngage;
        public UserInterfaceController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Update render filter menu on page (request from jquery )
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="filters">Set of filters</param>
        /// <param name="numberScroll"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FilterMenu(int numberScroll, int reportYear, IList<CheckListFilter> filters) {
            var findKrt = _bussinesEngage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).SingleOrDefault();

            //build lambda expression basic on active filter(linqKit)
            var finalPredicate = filters.Where(x => x.ActiveFilter).Aggregate(PredicateBuilder.True<krt_Naftan_orc_sapod>()
                .And(x => x.keykrt == findKrt.KEYKRT), (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()));

            //update filters base on active filter new values
            if (Request.IsAjaxRequest() && findKrt != null) {
                foreach (var item in filters) {
                    item.CheckedValues = _bussinesEngage.GetGroup(PredicateExtensions.GroupPredicate<krt_Naftan_orc_sapod>(item.SortFieldName).Expand(), finalPredicate.Expand());
                }

                //return Json(filters, JsonRequestBehavior.DenyGet);
                return PartialView("_FilterMenu", filters);
            }

            ModelState.AddModelError("Menu", @"Ошибка в работе фильтров!");
            TempData["message"] = @"Ошибка возникла в работе фильтров!";

            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
        }

        /// <summary>
        /// Quick pop-up menu. Main aim response for choose operation events
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult OperationQuickMenu() {
            var result = new[]
            {
                new QuickDialogMenuModel(){
                    HtmlAttrs = null,
                    Title = @"Найти соответствие",
                    Location = Url.Content("~/Content/Images/seo-chain-link-icon.png"),
                    OptionsAjax = new AjaxOptions() {
                        HttpMethod = "Post",
                        InsertionMode = InsertionMode.Replace,
                        UpdateTargetId = "operationScrollModal",
                        OnSuccess = "showModalResult"
                    },
                    RouteValues = new RouteValueDictionary() { { "actionName", "GeneralCorrection" }, { "controllerName", "Scroll" } },
                    TypeOperation = EnumMenuOperation.Join
                },
                new QuickDialogMenuModel(){
                    HtmlAttrs = null,
                    Title = "Редактировать",
                    Location = Url.Content("~/Content/Images/1474628461_pencil.png"),
                    OptionsAjax = new AjaxOptions() {
                        HttpMethod = "Post",
                        InsertionMode = InsertionMode.Replace,
                        UpdateTargetId = "operationScrollModal",
                        OnSuccess = "showModalResult"
                    },
                    RouteValues = new RouteValueDictionary() { { "actionName", "GeneralCorrection" }, { "controllerName", "Scroll" } },
                    TypeOperation = EnumMenuOperation.Edit
                },
                new QuickDialogMenuModel(){
                    HtmlAttrs = null,
                    Title = "Удалить",
                    Location = Url.Content("~/Content/Images/1474894106_scissors.png"),
                    OptionsAjax = new AjaxOptions() {
                        HttpMethod = "Post",
                        InsertionMode = InsertionMode.Replace,
                        UpdateTargetId = "operationScrollModal",
                        OnSuccess = "showModalResult"
                    },
                    RouteValues = new RouteValueDictionary() { { "actionName", "GeneralCorrection" }, { "controllerName", "Scroll" } },
                    TypeOperation = EnumMenuOperation.Delete
                },
            };

            return PartialView("_QuickMenu", result);
        }
    }
}