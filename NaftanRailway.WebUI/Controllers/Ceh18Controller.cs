using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    //[Authorize]
    //[HandleError(ExceptionType = typeof(ArgumentOutOfRangeException),View = "NomenclatureError",Master = "")] //Return HandleErrorInfo as model object
    //[HandleError(ExceptionType = typeof(ArgumentNullException), View = "NomenclatureErrorNull", Master = "")] //Return HandleErrorInfo as model object
    //[ExceptionFilter]
    public class Ceh18Controller : AsyncController {
        private readonly IRailwayModule _bussinesEngage;

        public Ceh18Controller(IRailwayModule bussinesEngage) {
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
            const short pageSize = 9;
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
                var result = _bussinesEngage.ShippingPreview(menuView.ShippingChoise, menuView.ReportPeriod, out recordCount);

                if (recordCount == 0) {
                    return PartialView("_NotFoundModal", menuView.ShippingChoise);
                }
                //report main date (month/year)
                ViewBag.datePeriod = menuView.ReportPeriod;
                return PartialView("_DeliveryPreviewModal", result.ToList());
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
        [HttpPost]
        public JsonResult BadgesCount(InputMenuViewModel menuView, EnumOperationType operationCategory) {

            //DateTime chooseDate = new DateTime(menuView.ReportPeriod.Year, menuView.ReportPeriod.Month, 1);

            //var resultGroup = _bussinesEngage.Badges(menuView.ShippingChoise, chooseDate, operationCategory);

            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add information about distach in db
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDocumentsInfo(SessionStorage storage, DateTime reportPeriod, IList<ShippingInfoLine> docInfo) {
            if (Request.IsAjaxRequest()) {
                _bussinesEngage.PackDocSql(reportPeriod, docInfo);

                return Index(storage, new InputMenuViewModel() { ReportPeriod = reportPeriod });
            }
            return new EmptyResult();
        }

        /// <summary>
        /// Delete information about invoice from database
        /// Action need some work about delete addional payment 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="reportPeriod"></param>
        /// <param name="idInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteDocInfo(SessionStorage storage, DateTime reportPeriod, int? idInvoice) {
            if (Request.IsAjaxRequest()) {
                TempData["message"] = (_bussinesEngage.DeleteInvoice(reportPeriod, idInvoice)) ? "Успех" : "Неудача";

                return Index(storage, new InputMenuViewModel() { ReportPeriod = reportPeriod });
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateExists(SessionStorage storage, DateTime reportPeriod) {
            if (Request.IsAjaxRequest()) {
                _bussinesEngage.UpdateExists(reportPeriod);
                return Index(storage, new InputMenuViewModel() { ReportPeriod = reportPeriod });
            }
            return new EmptyResult();
        }
    }
}