﻿using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    //[Authorize]
    public class Ceh18Controller : Controller {
        private readonly IBussinesEngage _bussinesEngage;

        public Ceh18Controller(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Main page with summary information
        /// </summary>
        /// <param name="storage">session storage</param>
        /// <param name="menuView">input menu</param>
        /// <param name="operationCategory">filter category</param>
        /// <param name="page">current page</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(SessionStorage storage, InputMenuViewModel menuView, EnumOperationType operationCategory = EnumOperationType.All, short page = 1) {
            const short pageSize = 10;
            short recordCount;

            menuView.ReportPeriod = _bussinesEngage.SyncActualDate(storage, menuView.ReportPeriod);

            var model = new DispatchListViewModel() {
                Dispatchs = _bussinesEngage.ShippingsViews(operationCategory, menuView.ReportPeriod, page, pageSize, out recordCount),
                PagingInfo = new PagingInfo() { CurrentPage = page, ItemsPerPage = pageSize, TotalItems = recordCount },
                OperationCategory = operationCategory,
                Menu = menuView
            };

            if (Request.IsAjaxRequest()) { return PartialView("ShippingSummary", model); }

            return View(model);
        }
        /// <summary>
        /// Action to responde to post request (for routing system actualy display selecting month)
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(InputMenuViewModel menuView) {          
            if (Request.IsAjaxRequest()) {
                short recordCount;
                var result = _bussinesEngage.PackDocuments(menuView.ShippingChoise, out recordCount);

                if (recordCount == 0) {
                    return PartialView("_NotFoundModal", result);
                }
                return PartialView("_DeliveryPreviewModal", result);
            }
            return new EmptyResult();
        }
        /// <summary>
        /// For shipping number autoComplete
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchNumberShipping(InputMenuViewModel menuView) {
            var result = _bussinesEngage.AutoCompleteShipping(menuView.ShippingChoise, menuView.ReportPeriod);

            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// Return grouping by oper result
        /// </summary>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult BadgesCount(InputMenuViewModel menuView, EnumOperationType operationCategory) {
        //    if(menuView.ReportPeriod != null) {
        //        DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

        //        var resultGroup = _bussinesEngage.Badges(menuView.ShippingChoise, chooseDate, operationCategory);

        //        return Json(resultGroup, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}
    }
}