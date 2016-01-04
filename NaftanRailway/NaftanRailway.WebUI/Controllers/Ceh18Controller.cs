using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.WebUI.Models;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    [Authorize]
    public class Ceh18Controller : Controller {
        /// <summary>
        /// Count visible elemnts per one page
        /// </summary>
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
        public ViewResult Index(SessionStorage storage, InputMenuViewModel menuView, EnumOperationType operationCategory = EnumOperationType.All, int page = 1) {
            const int pageSize = 8;
            const int shiftDay = 3;
            menuView.ShippingChoise = menuView.ShippingChoise ?? "";

            if(menuView.ReportPeriod == null) {
                menuView.ReportPeriod = storage.ReportPeriod;
            } else { storage.ReportPeriod = menuView.ReportPeriod.Value; }

            DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

            DispatchListViewModel model = new DispatchListViewModel() {
                Dispatchs = _bussinesEngage.ShippingsViews(menuView.ShippingChoise, operationCategory, chooseDate, page, pageSize, shiftDay),
                PagingInfo = new PagingInfo() {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = _bussinesEngage.ShippingsViewsCount(menuView.ShippingChoise, operationCategory, chooseDate)
                },
                OperationCategory = operationCategory,
                Menu = menuView
            };

            return View(model);
        }
        /// <summary>
        /// Action to responde to post request (for routing system actualy display selecting month)
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpPost]
        public RedirectToRouteResult Index(InputMenuViewModel menuView) {
            if(menuView.ShippingChoise == "") {
                return RedirectToRoute("Period", new {
                    reportPeriod = menuView.ReportPeriod != null ? menuView.ReportPeriod.Value.ToString("MMyyyy") : null,
                    page =1
                });
            }

            return RedirectToRoute("Path_Full", new {
                reportPeriod = menuView.ReportPeriod != null ? menuView.ReportPeriod.Value.ToString("MMyyyy") : null,
                templateNumber = menuView.ShippingChoise == "" ? null : menuView.ShippingChoise,
                page = 1
            });
        }
        /// <summary>
        /// For shipping number autoComplete
        /// </summary>
        /// <param name="menuView"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SearchNumberShipping(InputMenuViewModel menuView) {
            if(menuView.ReportPeriod != null) {
                DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

                IEnumerable<string> result = _bussinesEngage.AutoCompleteShipping(menuView.ShippingChoise, chooseDate);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Return grouping by oper result
        /// </summary>
        /// <param name="menuView"></param>
        /// <param name="operationCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BadgesCount(InputMenuViewModel menuView, EnumOperationType operationCategory) {
            if(menuView.ReportPeriod != null) {
                DateTime chooseDate = new DateTime(menuView.ReportPeriod.Value.Year, menuView.ReportPeriod.Value.Month, 1);

                var resultGroup = _bussinesEngage.Badges(menuView.ShippingChoise, menuView.ReportPeriod.Value, operationCategory);

                return Json(resultGroup, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}