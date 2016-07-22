using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
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

            DateTime chooseDate = new DateTime(menuView.ReportPeriod.Year, menuView.ReportPeriod.Month, 1);

            //var resultGroup = _bussinesEngage.Badges(menuView.ShippingChoise, chooseDate, operationCategory);

            return Json("", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add information about distach in db
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddDocumentsInfo(DateTime reportPeriod, IList<ShippingInfoLine> docInfo) {
            if (Request.IsAjaxRequest()) {
                //var strInfo = String.Join(", ", docInfo.Select(x => x.Shipping.n_otpr));
                //TempData["message"] = (_bussinesEngage.PackDocuments(reportPeriod, docInfo)) ? "Успешно добавлена информация по накладной(ым)" : "Ошибка добавления записей по накладной(ым)" + strInfo;
                _bussinesEngage.PackDocSQL(reportPeriod, docInfo);
            }
            return new EmptyResult();
        }
        /// <summary>
        /// Delete information about invoice from database
        /// </summary>
        /// <param name="reportPeriod"></param>
        /// <param name="idInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteDocInfo(DateTime reportPeriod, int idInvoice) {
            if (Request.IsAjaxRequest()) {
                TempData["message"] = (_bussinesEngage.DeleteInvoice(reportPeriod, idInvoice)) ? "Успех" : "Неудача";
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult UpdateExists(DateTime reportPeriod) {
            if (Request.IsAjaxRequest()) {
                _bussinesEngage.UpdateExists(reportPeriod);
            }
            return new EmptyResult();
        }

        public ActionResult Reports(DateTime reportPeriod) {
            if (Request.IsAjaxRequest()) {
                const string serverName = @"DB2";
                const string folderName = @"Orders";
                const string reportName = @"krt_Naftan_Guild18Report";

                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = @"reportPeriod=" + reportPeriod;

                string urlReportString = String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName,
                    folderName, reportName, defaultParameters, filterParameters);

                //WebClient client = new WebClient { UseDefaultCredentials = true };
                /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
                WebClient client = new WebClient {
                    Credentials = new CredentialCache{{new Uri("http://db2"), @"ntlm", new NetworkCredential(@"CPN", @"1111", @"LAN")}}
                };

                string nameFile =String.Format(@"Отчёт по провозным платежам и дополнительным сборам Бел. ж/д за {0}.xls",reportPeriod.ToString("M"));

                //Changing "attach;" to "inline;" will cause the file to open in the browser instead of the browser prompting to save the file.
                //encode the filename parameter of Content-Disposition header in HTTP (for support diffrent browser)
                string contentDisposition;
                if (Request.Browser.Browser == "IE" &&
                    (Request.Browser.Version == "7.0" || Request.Browser.Version == "8.0"))
                    contentDisposition = "attachment; filename=" + Uri.EscapeDataString(nameFile);
                else if (Request.Browser.Browser == "Safari")
                    contentDisposition = "attachment; filename=" + nameFile;
                else
                    contentDisposition = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(nameFile);

                //name file (with encoding)
                Response.AddHeader("Content-Disposition", contentDisposition);
                var returnFile = File(client.DownloadData(urlReportString), @"application/vnd.ms-excel");

                //For js spinner and complete donwload callback
                Response.Cookies.Clear();
                Response.AppendCookie(new HttpCookie("SSRSfileDownloadToken", "true"));

                return returnFile;
            }
            return new EmptyResult();
        }
    }
}