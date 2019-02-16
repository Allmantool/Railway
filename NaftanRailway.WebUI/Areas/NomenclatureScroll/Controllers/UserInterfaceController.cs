using System.Collections.Generic;
using System.Web.Mvc;
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
        private readonly INomenclatureModule _bussinesEngage;
        public UserInterfaceController(INomenclatureModule bussinesEngage) {
            this._bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Update render filter menu on page (request from jquery )
        /// </summary>
        /// <param name="reportYear"></param>
        /// <param name="filters">Set of filters</param>
        /// <param name="numberScroll"></param>
        /// <param name="typeFilter"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FilterMenu(int numberScroll, int reportYear, IList<CheckListFilter> filters, EnumTypeFilterMenu typeFilter) {

            var findKrt = this._bussinesEngage.GetNomenclatureByNumber(numberScroll, reportYear);

            //update filters base on active filter new values
            if (this.Request.IsAjaxRequest() && this._bussinesEngage.UpdateRelatingFilters(findKrt, ref filters, typeFilter)) {
                //return Json(filters, JsonRequestBehavior.DenyGet);
                return this.PartialView("_FilterMenu", filters);
            }

            this.ModelState.AddModelError("Menu", @"Ошибка в работе фильтров!");
            this.TempData["message"] = @"Ошибка возникла в работе фильтров!";

            return this.RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", 1 } });
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
                    Title = @"Найти соответствие в САПОД",
                    Location = this.Url.Content("~/Content/Images/seo-chain-link-icon.png"),
                    RouteValues = new RouteValueDictionary() { { "actionName", "GeneralCorrection" }, { "controllerName", "Scroll" } },
                    TypeOperation = EnumMenuOperation.Join
                },
                new QuickDialogMenuModel(){
                    HtmlAttrs = null,
                    Title = "Редактировать",
                    Location = this.Url.Content("~/Content/Images/1474628461_pencil.png"),
                    RouteValues = new RouteValueDictionary() { { "actionName", "GeneralCorrection" }, { "controllerName", "Scroll" } },
                    TypeOperation = EnumMenuOperation.Edit
                },
                new QuickDialogMenuModel(){
                    HtmlAttrs = null,
                    Title = "Удалить",
                    Location = this.Url.Content("~/Content/Images/1474894106_scissors.png"),
                    RouteValues = new RouteValueDictionary() { { "actionName", "GeneralCorrection" }, { "controllerName", "Scroll" } },
                    TypeOperation = EnumMenuOperation.Delete
                },
            };

            return this.PartialView("_QuickMenu", result);
        }
    }
}