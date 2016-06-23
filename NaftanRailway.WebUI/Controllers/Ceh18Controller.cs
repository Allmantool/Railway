﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
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
            //reload page (save select report date)
            if (storage.ReportPeriod == DateTime.MinValue && menuView.ReportPeriod == DateTime.MinValue) {
                menuView.ReportPeriod = DateTime.Today;
            } else if (storage.ReportPeriod != DateTime.MinValue && menuView.ReportPeriod == DateTime.MinValue) {
                menuView.ReportPeriod = storage.ReportPeriod;
            } else {
                storage.ReportPeriod = menuView.ReportPeriod;
            }

            DateTime chooseDate = new DateTime(menuView.ReportPeriod.Year, menuView.ReportPeriod.Month, 1);

            var model = new DispatchListViewModel() {
                Dispatchs = _bussinesEngage.ShippingsViews(operationCategory, chooseDate, page, pageSize, out recordCount),
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
        public RedirectToRouteResult Index(InputMenuViewModel menuView) {
            if (menuView.ShippingChoise == "") {
                return RedirectToRoute("Period", new {
                    reportPeriod = menuView.ReportPeriod != null ? menuView.ReportPeriod.ToString("MMyyyy") : null,
                    page = 1
                });
            }

            return RedirectToRoute("Path_Full", new {
                reportPeriod = menuView.ReportPeriod != null ? menuView.ReportPeriod.ToString("MMyyyy") : null,
                templateNumber = menuView.ShippingChoise == "" ? null : menuView.ShippingChoise,
                page = 1
            });
        }
        /// <summary>
        /// For shipping number autoComplete
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult SearchNumberShipping(InputMenuViewModel menuView) {
            if (menuView.ReportPeriod == null) return Json("", JsonRequestBehavior.AllowGet);

            DateTime chooseDate = new DateTime(menuView.ReportPeriod.Year, menuView.ReportPeriod.Month, 1);
            IEnumerable<string> result = _bussinesEngage.AutoCompleteShipping(menuView.ShippingChoise, chooseDate);

            return Json(result, JsonRequestBehavior.AllowGet);
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