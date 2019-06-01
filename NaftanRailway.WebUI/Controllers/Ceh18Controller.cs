using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Services;
using NaftanRailway.WebUI.ViewModels;
using NaftanRailway.BLL.DTO.Guild18;
using NaftanRailway.WebUI.Infrastructure.Filters;
using log4net;

namespace NaftanRailway.WebUI.Controllers {
    //[Authorize(Roles = "LAN\\cpn, cpn", Users = "LAN\\cpn, cpn,Чижиков П.Н.")]
    [AuthorizeAD(Groups = "Rail_Developers, Rail_Users, Администраторы", DenyUsers = @"lan\snn, lan\kpg")]
    //[HandleError(ExceptionType = typeof(ArgumentOutOfRangeException),View = "NomenclatureError",Master = "")] //Return HandleErrorInfo as model object
    //[HandleError(ExceptionType = typeof(ArgumentNullException), View = "NomenclatureErrorNull", Master = "")] //Return HandleErrorInfo as model object
    //[ExceptionFilter]
    //[NonController]
    public class Ceh18Controller : BaseController {
        private readonly IRailwayModule _bussinesEngage;

        public Ceh18Controller(IRailwayModule bussinesEngage, ILog logger) : base(logger) {
            this._bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// Main page with summary information
        /// </summary>
        [HttpGet]
        public ActionResult Index(
            SessionStorage storage,
            InputMenuViewModel menuView,
            EnumOperationType operationCategory = EnumOperationType.All,
            short page = 1,
            short pageSize = 9,
            bool asService = false) {

            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                menuView.ReportPeriod = this._bussinesEngage.SyncActualDate(storage, menuView.ReportPeriod);

                var typeOfOperation = this.Request.QueryString["operationCategory"] == string.Empty
                    ? (int)EnumOperationType.All
                    : int.Parse(this.Request.QueryString["operationCategory"]);

                var model = new DispatchListViewModel {
                    Dispatchs = this._bussinesEngage.ShippingsViews(
                        (EnumOperationType)typeOfOperation,
                        menuView.ReportPeriod,
                        page,
                        pageSize,
                        out var recordCount),
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = pageSize,
                        TotalItems = recordCount,
                        RoutingDictionary = this.Request.RequestContext.RouteData.Values
                    },
                };

                if (asService) {
                    return this.Json(model, JsonRequestBehavior.AllowGet);
                }

                return this.PartialView("ShippingSummary", model);
            }

            return this.View();
        }

        /// <summary>
        /// Action to response to post request (for routing system actually display selecting month)
        /// </summary>
        /// <param name="menuView"></param>
        /// <param name="asService"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(InputMenuViewModel menuView, bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid) {
                short recordCount;
                var result = this._bussinesEngage.ShippingPreview(menuView.ShippingChoise, menuView.ReportPeriod, out recordCount);

                if (asService) {
                    return this.Json(result, JsonRequestBehavior.DenyGet);
                }
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

            var result = this._bussinesEngage.AutoCompleteShipping(menuView.ShippingChoise, menuView.ReportPeriod);

            return this.Json(result, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// Add information about distach to db
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDocumentsInfo(SessionStorage storage, DateTime reportPeriod, IList<ShippingInfoLineDTO> docInfo, bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid && asService) {
                var result = this._bussinesEngage.PackDocSql(reportPeriod, docInfo);

                return this.Json(result, JsonRequestBehavior.DenyGet);
            }

            return this.Index(storage, new InputMenuViewModel() { ReportPeriod = reportPeriod }, asService: asService);
        }

        /// <summary>
        /// Delete information about invoice from database
        /// Action need some work about delete additional payment
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="reportPeriod"></param>
        /// <param name="idInvoice"></param>
        /// <param name="asService"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteDocInfo(SessionStorage storage, DateTime reportPeriod, int? idInvoice, bool asService = false) {
            if (Request.IsAjaxRequest() && ModelState.IsValid && asService) {
                //TempData["message"] = (_businessProvider.DeleteInvoice(reportPeriod, idInvoice)) ? "Успех" : "Неудача";
                var result = _bussinesEngage.DeleteInvoice(reportPeriod, idInvoice);

                return this.Json(result, JsonRequestBehavior.DenyGet);
            }

            return this.Index(storage, new InputMenuViewModel() { ReportPeriod = reportPeriod }, asService: asService);
        }

        [HttpPost]
        public ActionResult UpdateExists(SessionStorage storage, DateTime reportPeriod, bool asService = false) {
            if (this.Request.IsAjaxRequest() && this.ModelState.IsValid && asService) {
                var result = this._bussinesEngage.UpdateExists(reportPeriod);

                return this.Json(result, JsonRequestBehavior.DenyGet);
            }

            return this.Index(storage, new InputMenuViewModel() { ReportPeriod = reportPeriod }, asService: asService);
        }

        /// <summary>
        /// OVerview estimated carriages
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[NonAction]
        public JsonResult Overview() {
            var result = this._bussinesEngage.EstimatedCarrieages();

            return this.Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}