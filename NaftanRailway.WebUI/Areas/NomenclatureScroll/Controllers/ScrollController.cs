﻿using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    public class ScrollController : Controller {
        private readonly IBussinesEngage _bussinesEngage;

        public ScrollController(IBussinesEngage bussinesEngage) {
            _bussinesEngage = bussinesEngage;
        }

        /// <summary>
        /// View table krt_Naftan (with infinite scrolling)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(int page = 0) {
            const byte initialSizeItem = 27;

            if(Request.IsAjaxRequest()) {
                return PartialView("_AjaxKrtNaftanRow", _bussinesEngage.GetTable<krt_Naftan>()
                    .OrderByDescending(x => x.KEYKRT).Skip(page*initialSizeItem).Take(initialSizeItem));
            }

            return View(new IndexModelView() {
                ListKrtNaftan = _bussinesEngage.GetTable<krt_Naftan>()
                    .OrderByDescending(x => x.KEYKRT).Skip(page*initialSizeItem).Take(initialSizeItem),
                ReportPeriod = DateTime.Now
            });
        }

        /// <summary>
        /// Change Buh Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeData(IndexModelView model) {
            long numberKeykrt = model.ListKrtNaftan.First().KEYKRT;

            if(model.ReportPeriod != null &&
                (Request.IsAjaxRequest() && _bussinesEngage.ChangeBuhDate(model.ReportPeriod.Value, numberKeykrt))) {
                return PartialView("_AjaxKrtNaftanRow",
                    _bussinesEngage.GetTable<krt_Naftan>()
                        .Where(x => x.KEYKRT >= numberKeykrt)
                        .OrderByDescending(x => x.KEYKRT));
            }

            return RedirectToAction("Index", "Scroll");
        }

        /// <summary>
        /// Request from ajax-link and then response json to JqueryFunction(UpdateData)
        /// </summary>
        /// <param name="scrollKey"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Confirmed(long? scrollKey) {

            var selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == scrollKey).FirstOrDefault();

            if(ModelState.IsValid && selectKrt != null && _bussinesEngage.AddKrtNaftan(selectKrt.KEYKRT)) {
                //return Json(selectKrt, "application/json", JsonRequestBehavior.DenyGet);
                return PartialView(@"~/Areas/NomenclatureScroll/Views/Shared/_AjaxKrtNaftanRow.cshtml",
                    new[] { selectKrt });
                //return RedirectToAction("ErrorReport", "Scroll",new RouteValueDictionary() { {"numberKrt",numberKeykrt},{"reportYear",selectKrt.DTBUHOTCHET.Year}});
            }

            TempData["message"] = String.Format(@"Ошибка добавления перечень № {0}.Вероятно, он уже добавлен",
                selectKrt.KEYKRT);

            return RedirectToAction("Index", "Scroll");
        }

        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// Detail gathering of one scroll 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScrollDetails(long? scrollKey) {
            if(scrollKey != null &&
                _bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.keykrt == scrollKey).FirstOrDefault() != null) {
                return View(_bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.keykrt == scrollKey));
            }

            TempData["message"] = String.Format(@"Для получения информации укажите подтвержденный перечень!");

            return RedirectToAction("Index", "Scroll");
        }

        /// <summary>
        /// Render Report error
        /// rs:sent command Report Server (RS)
        /// rc:provides device-information settings based on the report's output format
        /// rv:pass parameters to reports that are stored in a Sharepoint document Library
        /// dsu:(username) or dsp:(password) sent database credentials
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ErrorReport(string reportName, long? numberKrt, int? reportYear) {
            const string serverName = @"DB2";
            const string folderName = @"Orders";

            if(reportName != null) {
                string urlReportString = string.Format(@"http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}",
                    serverName, folderName,
                    reportName,
                    @"rs:Command=Render");
                return View((object)urlReportString);
            }

            krt_Naftan selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == numberKrt).FirstOrDefault();

            if(selectKrt != null) {
                reportName = selectKrt.SignAdjustment_list
                    ? @"krt_Naftan_Scroll_compare_Correction"
                    : @"krt_Naftan_Scroll_Compare_Normal";
                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = @"nkrt=" + selectKrt.NKRT + @"&y=" + reportYear;

                string urlReportString = String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName,
                    folderName, reportName, defaultParameters, filterParameters);

                //WebClient client = new WebClient { UseDefaultCredentials = true };
                /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
                WebClient client = new WebClient {
                    Credentials =
                        new CredentialCache
                        {
                            {
                                new Uri("http://db2"),
                                @"ntlm",
                                new NetworkCredential(@"CPN", @"1111", @"LAN")
                            }
                        }
                };

                return File(client.DownloadData(urlReportString), @"application/vnd.ms-excel",
                    String.Format(@"Отчёт по переченю №{0}.xls", selectKrt.NKRT));
            }

            TempData[@"message"] = String.Format(@"Невозможно вывести отчёт. Ошибка! Возможно не указан перечень");
            return RedirectToAction("Index", "Scroll");
        }

        /// <summary>
        /// Return Bookkeeper Report
        /// </summary>
        /// <param name="numberKrt"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BookkeeperReport(long? numberKrt) {
            const string serverName = @"DB2";
            const string folderName = @"Orders";
            const string reportName = @"";
            const string reportYear = @"";
            krt_Naftan selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == numberKrt).FirstOrDefault();

            if(selectKrt != null) {
                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = @"nkrt=" + selectKrt.NKRT + @"&y=" + reportYear;

                string urlReportString = String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName,
                    folderName, reportName, defaultParameters, filterParameters);

                //WebClient client = new WebClient { UseDefaultCredentials = true };
                /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
                WebClient client = new WebClient {
                    Credentials =
                        new CredentialCache{{
                                new Uri("http://db2"),
                                @"ntlm",
                                new NetworkCredential(@"CPN", @"1111", @"LAN")
                            }
                        }
                };

                return File(client.DownloadData(urlReportString), @"application/vnd.ms-excel",
                    String.Format(@"Отчёт по переченю №{0}.xls", selectKrt.NKRT));
            }

            return RedirectToAction("Index", "Scroll");
        }
    }
}