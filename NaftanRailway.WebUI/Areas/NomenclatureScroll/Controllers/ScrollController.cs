﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LinqKit;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    //[SessionState(SessionStateBehavior.Disabled)]
    public class ScrollController : AsyncController {
        private readonly INomenclatureModule _bussinesEngage;
        public ScrollController(INomenclatureModule bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }
        /// <summary>
        /// View table krt_Naftan (with infinite scrolling)
        /// For increase jquery perfomance in IE8 method apply paging instead of ajax infinite scrolling
        /// (IsAjaxRequest leave for compability with older version (ajax)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[ActionName("Enumerate")]
        public ActionResult Index(int page = 1) {
            const byte initialSizeItem = 100;
            long recordCount;

            var result = new IndexModelView() {
                ListKrtNaftan = _bussinesEngage.SkipScrollTable(page, initialSizeItem, out recordCount),
                ReportPeriod = DateTime.Now,
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = initialSizeItem,
                    TotalItems = recordCount
                }
            };
            if (Request.IsAjaxRequest()) {
                return PartialView("_AjaxTableKrtNaftan", result);
            }
            return View(result);
        }
        /// <summary>
        /// Change Buh Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeDate(IndexModelView model) {
            //Custom value provider binding => TryUpdateModel(model, new FormValueProvider(ControllerContext));
            if (Request.IsAjaxRequest()) {
                return PartialView("_KrtNaftanRows", _bussinesEngage.ChangeBuhDate(model.ReportPeriod, model.Nkrt, model.MultiDate));
            }

            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }
        /// <summary>
        /// Get nomenclature from ORC
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdmitScroll() {
            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                _bussinesEngage.SyncWithOrc();
                return Index();
            }
            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }
        /// <summary>
        /// Request from ajax-link and then response json to JqueryFunction(UpdateData)
        /// </summary>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirmed(int numberScroll, int reportYear) {
            string msgError = "";

            if (Request.IsAjaxRequest() && ModelState.IsValid) {
                //return Json(selectKrt, "application/json", JsonRequestBehavior.DenyGet);
                return PartialView("_KrtNaftanRows", new List<krt_Naftan>() { _bussinesEngage.AddKrtNaftan(numberScroll, reportYear, out msgError) });
            }

            TempData["message"] = String.Format(@"Ошибка добавления переченя № {0}. {1}", numberScroll, msgError);

            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }
        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// Detail gathering of one scroll 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScrollDetails(int numberScroll, int reportYear, int page = 1, byte initialSizeItem = 80) {
            var findKrt = _bussinesEngage.Engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).SingleOrDefault();

            if (findKrt != null) {
                var result = new DetailModelView() {
                    Scroll = findKrt,
                    Filters = new[]{ new CheckListFilterModel(_bussinesEngage.Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.nkrt,x => x.keykrt == findKrt.KEYKRT))
                        {SortFieldName = "nkrt",NameDescription = "Накоп. Карточки:"},
                        new CheckListFilterModel(_bussinesEngage.Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.tdoc.ToString(),x => x.keykrt == findKrt.KEYKRT))
                        {SortFieldName = "tdoc",NameDescription = "Тип документа:"},
                        new CheckListFilterModel(_bussinesEngage.Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.vidsbr.ToString(),x => x.keykrt == findKrt.KEYKRT))
                        {SortFieldName = "vidsbr",NameDescription = "Вид сбора:"},
                        new CheckListFilterModel(_bussinesEngage.Engage.GetGroup<krt_Naftan_orc_sapod, string>(x => x.nomot.ToString(),x => x.keykrt == findKrt.KEYKRT))
                        {SortFieldName = "nomot",NameDescription = "Документ:"}
                    },
                    PagesInfo = new PagingInfo {
                        CurrentPage = page,
                        ItemsPerPage = initialSizeItem,
                        TotalItems = findKrt.RecordCount
                    },
                    CollDetails = _bussinesEngage.Engage.GetSkipRows<krt_Naftan_orc_sapod, object>(page, initialSizeItem,
                                    x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt },
                                    x => x.keykrt == findKrt.KEYKRT)
                };

                if (result.CollDetails.Any()) {
                    if (Request.IsAjaxRequest()) {
                        return PartialView("_AjaxTableKrtNaftan_ORC_SAPOD", result);
                    }

                    return View(result);
                }
            }
            ModelState.AddModelError("Confirmed", @"Для получения информации укажите подтвержденный перечень!");
            TempData["message"] = @"Для получения информации укажите подтвержденный перечень!";

            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", page } });
        }
        [HttpPost]
        public ActionResult ScrollDetails(int numberScroll, int reportYear, List<CheckListFilterModel> filters, int page = 1, byte initialSizeItem = 80) {
            var findKrt = _bussinesEngage.Engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).FirstOrDefault();

            if (findKrt != null) {
                //upply filters(linqKit)
                var finalPredicate = filters.Aggregate(PredicateBuilder.True<krt_Naftan_orc_sapod>()
                    .And(x => x.keykrt == findKrt.KEYKRT), (current, innerItemMode) => current.And(innerItemMode.FilterByField<krt_Naftan_orc_sapod>()));

                var result = new DetailModelView() {
                    Scroll = findKrt,
                    PagesInfo = new PagingInfo {
                        CurrentPage = page,
                        ItemsPerPage = initialSizeItem,
                        TotalItems = findKrt.RecordCount
                    },
                    CollDetails = _bussinesEngage.Engage.GetSkipRows<krt_Naftan_orc_sapod, object>(page, initialSizeItem, x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt }, finalPredicate.Expand())
                };

                if (result.CollDetails.Any()) {
                    if (Request.IsAjaxRequest()) {
                        return PartialView("_KrtNaftan_ORC_SAPODRows", result.CollDetails);
                    }

                    return View("ScrollDetails", result);
                }
            }
            ModelState.AddModelError("Confirmed", @"Для получения информации укажите подтвержденный перечень!");
            TempData["message"] = @"Для получения информации укажите подтвержденный перечень!";

            return RedirectToAction("Index", "Scroll", new RouteValueDictionary() { { "page", page } });
        }
        /// <summary>
        /// Fix scroll row on side of Sapod
        /// </summary>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScrollCorrection(int numberScroll, int reportYear, int page = 1) {
            const byte initialSizeItem = 47;
            var findKrt = _bussinesEngage.Engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).SingleOrDefault();

            if (findKrt != null) {
                var fixRow = _bussinesEngage.Engage.GetSkipRows<krt_Naftan_orc_sapod, object>(page, initialSizeItem, x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt }, x => x.keykrt == findKrt.KEYKRT && (x.sm != (x.summa + x.nds) || x.sm_nds != x.nds));

                if (fixRow.Any()) {
                    var result = new DetailModelView() {
                        Title = String.Format(@"Корректировка записей перечня №{0}", numberScroll),
                        Scroll = findKrt,
                        PagesInfo = new PagingInfo {
                            CurrentPage = page,
                            ItemsPerPage = initialSizeItem,
                            TotalItems = fixRow.Count()
                        }
                    };

                    return View("_AjaxTableKrtNaftan_ORC_SAPOD", result);
                }
            }
            TempData["message"] = String.Format(@"Перечень №{0} не нуждается в корректировке!", numberScroll);

            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }
        /// <summary>
        /// Edit (nds and summa)
        /// If count fix rows better then 0 then display partial view with them, anothor hand redirect to main page (index)
        /// </summary>
        /// <param name="smNds"></param>
        /// <param name="nds"></param>
        /// <param name="sm"></param>
        /// <param name="summa"></param>
        /// <param name="nomot"></param>
        /// <param name="vidsbr"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ScrollCorrection(decimal smNds, decimal nds, decimal sm, decimal summa, string nomot, int vidsbr, int page = 1) {
            const byte initialSizeItem = 47;

            var corretionItem = _bussinesEngage.Engage.GetTable<krt_Naftan_orc_sapod, long>(x => x.nomot == nomot && x.vidsbr == vidsbr && x.sm == sm && x.sm_nds == smNds).FirstOrDefault();
            string numberScroll = _bussinesEngage.Engage.GetTable<krt_Naftan, long>(x => x.KEYKRT == corretionItem.keykrt).SingleOrDefault().NKRT.ToString();

            if (Request.IsAjaxRequest() && corretionItem != null) {
                _bussinesEngage.EditKrtNaftanOrcSapod(corretionItem.keykrt, corretionItem.keysbor, nds, summa);
                //Trouble
                var fixRow = _bussinesEngage.Engage.GetTable<krt_Naftan_orc_sapod, object>(x => x.keykrt == corretionItem.keykrt && (x.sm != (x.summa + x.nds) || x.sm_nds != x.nds), x => new { x.nkrt, x.tdoc, x.vidsbr, x.dt }).ToList();
                var result = new DetailModelView() { };
                //Info about paging
                ViewBag.Title = String.Format(@"Корректировка записей перечня №{0}.", numberScroll);
                ViewBag.PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = initialSizeItem,
                    TotalItems = fixRow.Count()
                };

                if (!fixRow.Any())
                    TempData["message"] = String.Format(@"Перечень №{0} не нуждается в корректировке!", numberScroll);

                return (fixRow.Any()) ? (ActionResult)PartialView("_AjaxTableKrtNaftan_ORC_SAPOD", result) : RedirectToAction("Index", "Scroll");

            }
            return RedirectToAction("Index", "Scroll", new { page = 1 });
        }
        /// <summary>
        /// General method for donwload files or display report throught SSRS
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        //[ChildActionOnly]
        //[FileDownloadCompleteFilter]
        public ActionResult Reports(string reportName, int? numberScroll, int? reportYear) {
            const string serverName = @"DB2";
            const string folderName = @"Orders";

            //link to SSRS buil-in repors
            if (numberScroll == null || reportYear == null) {
                string urlReportString = string.Format(@"http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}",
                    serverName, folderName, reportName, @"rs:Command=Render");

                return View("Reports", (object)urlReportString);
            }
            var selScroll = _bussinesEngage.Engage.GetTable<krt_Naftan, long>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).FirstOrDefault();
            //check exists
            if (selScroll != null) {
                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = (reportName == @"krt_Naftan_act_of_Reconciliation") ?
                      @"month=" + selScroll.DTBUHOTCHET.Month + @"&year=" + selScroll.DTBUHOTCHET.Year
                    : @"nkrt=" + numberScroll + @"&year=" + reportYear;

                string urlReportString = String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName,
                    folderName, reportName, defaultParameters, filterParameters);

                //WebClient client = new WebClient { UseDefaultCredentials = true };
                /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
                WebClient client = new WebClient {
                    Credentials =
                        new CredentialCache{
                            {new Uri("http://db2"),@"ntlm",new NetworkCredential(@"CPN", @"1111", @"LAN")}
                        }
                };

                string nameFile = (reportName == @"krt_Naftan_BookkeeperReport"
                    ? String.Format(@"Бухгалтерский отчёт по переченю №{0}.xls", numberScroll) : (reportName == @"krt_Naftan_act_of_Reconciliation")
                    ? String.Format(@"Реестр электронного  представления перечней ОРЦ за {0} {1} года.xls", selScroll.DTBUHOTCHET.ToString("MMMM"), selScroll.DTBUHOTCHET.Year)
                        : String.Format(@"Отчёт о ошибках по переченю №{0}.xls", numberScroll));

                //Changing "attach;" to "inline;" will cause the file to open in the browser instead of the browser prompting to save the file.
                //encode the filename parameter of Content-Disposition header in HTTP (for support diffrent browser)
                string contentDisposition;
                if (Request.Browser.Browser == "IE" && (Request.Browser.Version == "7.0" || Request.Browser.Version == "8.0"))
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
            TempData[@"message"] = (@"Невозможно вывести отчёт. Ошибка! Возможно не указан перечень");

            return RedirectToAction("Index", "Scroll");
        }
    }
}