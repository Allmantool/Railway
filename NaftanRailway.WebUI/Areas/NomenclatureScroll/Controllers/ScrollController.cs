using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.Areas.NomenclatureScroll.Models;
using NaftanRailway.WebUI.Infrastructure.Filters;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {

    //[SessionState(SessionStateBehavior.ReadOnly)] 
    public class ScrollController : AsyncController {

        private readonly IBussinesEngage _bussinesEngage;

        public ScrollController(IBussinesEngage bussinesEngage) {
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
            const byte initialSizeItem = 47;
            int recordCount = _bussinesEngage.GetTable<krt_Naftan>().Count();

<<<<<<< HEAD
            if (page >= 1 && page <= recordCount) {
                if (Request.IsAjaxRequest()) {
=======
            if(page >= 1 && page <= recordCount) {
                if(Request.IsAjaxRequest()) {
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
                    return new EmptyResult();
                    //    return PartialView("_AjaxKrtNaftanRow", _bussinesEngage.GetTable<krt_Naftan>()
                    //        .OrderByDescending(x => x.KEYKRT).Skip((page-1)*initialSizeItem).Take(initialSizeItem));
                }

                return View(new IndexModelView() {
                    ListKrtNaftan = _bussinesEngage.GetTable<krt_Naftan>()
<<<<<<< HEAD
                        .OrderByDescending(x => x.KEYKRT).Skip((page - 1) * initialSizeItem).Take(initialSizeItem),
=======
                        .OrderByDescending(x => x.KEYKRT).Skip((page - 1)*initialSizeItem).Take(initialSizeItem),
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
                    ReportPeriod = DateTime.Now,
                    PagingInfo = new PagingInfo {
                        CurrentPage = page,
                        ItemsPerPage = initialSizeItem,
                        TotalItems = recordCount
                    }
                });
            }

            return RedirectToAction("Index", new RouteValueDictionary() { { "page", 1 } });
        }

        /// <summary>
        /// Change Buh Data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeData(IndexModelView model) {
            long numberKeykrt = model.ListKrtNaftan.First().KEYKRT;

            if (model.ReportPeriod != null &&
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
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
        [HttpPost]
<<<<<<< HEAD
        public ActionResult Confirmed(long? scrollKey) {
            var selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == scrollKey).FirstOrDefault();
=======
        public ActionResult Confirmed(int? numberScroll, int? reportYear) {
            var selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear).FirstOrDefault();
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c

            if (Request.IsAjaxRequest() && ModelState.IsValid && selectKrt != null && _bussinesEngage.AddKrtNaftan(selectKrt.KEYKRT)) {
                //return Json(selectKrt, "application/json", JsonRequestBehavior.DenyGet);
<<<<<<< HEAD
                return PartialView(@"~/Areas/NomenclatureScroll/Views/Shared/_AjaxKrtNaftanRow.cshtml", new[] { selectKrt });
                //return RedirectToAction("ErrorReport", "Scroll",new RouteValueDictionary() { {"numberKrt",scrollKey}{"reportYear",selectKrt.DTBUHOTCHET.Year}});
=======
                return PartialView(@"~/Areas/NomenclatureScroll/Views/Shared/_AjaxKrtNaftanRow.cshtml",new[] { selectKrt });
                //return RedirectToAction("ErrorReport", "Scroll",new RouteValueDictionary() { {"numberKrt",scrollKey},{"reportYear",selectKrt.DTBUHOTCHET.Year}});
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
            }

            TempData["message"] = String.Format(@"Ошибка добавления перечень № {0}.Вероятно, он уже добавлен",numberScroll);

            return RedirectToAction("Index", "Scroll");
        }

        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// Detail gathering of one scroll 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ScrollDetails(int? numberScroll, int? reportYear, int page = 1) {
            const byte initialSizeItem = 47;

<<<<<<< HEAD
            if (scrollKey != null && _bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.keykrt == scrollKey).FirstOrDefault() != null) {
                int recordCount = _bussinesEngage.GetTable<krt_Naftan_orc_sapod>().Count(x => x.keykrt == scrollKey);
                if (Request.IsAjaxRequest()) {
                    return PartialView("_AjaxKrtNaftan_ORC_SAPOD_Row", _bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.keykrt == scrollKey)
                        .OrderByDescending(x => new { x.keykrt, x.nomot, x.keysbor }).Skip((page) * initialSizeItem).Take(initialSizeItem));
=======
            if((numberScroll != null || reportYear !=null) &&
                _bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.nper ==numberScroll && x.DtBuhOtchet.Year == reportYear).FirstOrDefault() != null) {
                int recordCount = _bussinesEngage.GetTable<krt_Naftan_orc_sapod>().Count(x => x.nper ==numberScroll && x.DtBuhOtchet.Year == reportYear);
                if(Request.IsAjaxRequest()) {
                    return PartialView("_AjaxKrtNaftan_ORC_SAPOD_Row",
                        _bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.nper ==numberScroll && x.DtBuhOtchet.Year == reportYear)
                            .OrderByDescending(x => new { x.keykrt, x.nomot, x.keysbor })
                            .Skip((page)*initialSizeItem)
                            .Take(initialSizeItem));
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
                }
                //Info about paging
                ViewBag.PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = initialSizeItem,
                    TotalItems = recordCount
                };
<<<<<<< HEAD
                return View(_bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.keykrt == scrollKey)
                    .OrderByDescending(x => x.keykrt).ThenBy(z => z.nomot).ThenBy(y => y.keysbor).Skip((page - 1) * initialSizeItem).Take(initialSizeItem));
=======
                return View(_bussinesEngage.GetTable<krt_Naftan_orc_sapod>(x => x.nper ==numberScroll && x.DtBuhOtchet.Year == reportYear)
                    .OrderByDescending(x => x.keykrt)
                    .ThenBy(z => z.nomot)
                    .ThenBy(y => y.keysbor)
                    .Skip((page - 1)*initialSizeItem)
                    .Take(initialSizeItem));
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
            }

            TempData["message"] = String.Format(@"Для получения информации укажите подтвержденный перечень!");

            return RedirectToAction("Index", "Scroll");
        }

<<<<<<< HEAD
        /// <summary>
        /// Render Report error
        /// rs:sent command Report Server (RS)
        /// rc:provides device-information settings based on the report's output format
        /// rv:pass parameters to reports that are stored in a Sharepoint document Library
        /// dsu:(username) or dsp:(password) sent database credentials
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FileDownloadCompleteFilter]
        public ActionResult ErrorReport(string reportName, long? numberKrt, int? reportYear) {
            const string serverName = @"desktop-lho63th";//@"DB2";
            const string folderName = @"Orders";

            if (reportName != null) {
                string urlReportString = string.Format(@"http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}",
                    serverName, folderName,
                    reportName,
                    @"rs:Command=Render");
                return View((object)urlReportString);
            }
=======
        ///// <summary>
        ///// Render Report error
        ///// rs:sent command Report Server (RS)
        ///// rc:provides device-information settings based on the report's output format
        ///// rv:pass parameters to reports that are stored in a Sharepoint document Library
        ///// dsu:(username) or dsp:(password) sent database credentials
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        ////[FileDownloadCompleteFilter]
        //public ActionResult ErrorReport(string reportName, long? numberKrt, int? reportYear) {
        //    krt_Naftan selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == numberKrt).FirstOrDefault();

        //    if (reportName != ""){
        //       return RedirectToAction("DonwloadFile","Scroll",new RouteValueDictionary(){{"reportName",reportName}}); 
        //    }
        //    if(selectKrt != null) {
        //        reportName = selectKrt.SignAdjustment_list
        //            ? @"krt_Naftan_Scroll_compare_Correction"
        //            : @"krt_Naftan_Scroll_Compare_Normal";
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c

        //        return RedirectToAction("DonwloadFile","Scroll",new RouteValueDictionary(){{"reportName",reportName}});
        //    }

<<<<<<< HEAD
            if (selectKrt != null) {
                reportName = selectKrt.SignAdjustment_list
                    ? @"krt_Naftan_Scroll_compare_Correction"
                    : @"krt_Naftan_Scroll_Compare_Normal";
                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = @"nkrt=" + selectKrt.NKRT + @"&y=" + reportYear;
=======
        //    TempData[@"message"] = String.Format(@"Невозможно вывести отчёт. Ошибка! Возможно не указан перечень");
        //    return RedirectToAction("Index", "Scroll");
        //}
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c

        ///// <summary>
        ///// Return Bookkeeper Report (some dublicate code)
        ///// </summary>
        ///// <param name="reportName"></param>
        ///// <param name="numberKrt"></param>
        ///// <param name="reportYear"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult BookkeeperReport(string reportName, long? numberKrt, int? reportYear) {
        //    const string serverName = @"DB2";
        //    const string folderName = @"Orders";
        //    reportName = "krt_Naftan_BookkeeperReport";
        //    krt_Naftan selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == numberKrt).FirstOrDefault();

<<<<<<< HEAD
                WebClient client = new WebClient { UseDefaultCredentials = true };
                /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
                //WebClient client = new WebClient {
                //    Credentials =
                //        new CredentialCache{{
                //                new Uri("http://db2"),
                //                @"ntlm",
                //                new NetworkCredential(@"CPN", @"1111", @"LAN")
                //            }
                //        },

                //};
                //byte[] buffer = client.DownloadData(urlReportString);
                //Response.Clear();
                //Response.ClearHeaders();
                //Response.Buffer = true;
                //Response.ContentType = "application/text";
                //Response.AddHeader("Content-Disposition", @"filename=""IT Report.xls""");
                //Response.BinaryWrite(buffer);
                //Response.Flush();
                //Необходимо указывать расположение на сервере (от куда будет скачиваться файл(server's physical location.))
                //Response.TransmitFile(@"c:\Users\AllmanGroup\Desktop\income_tax_report.xls");
                //return new EmptyResult();
                return File(client.DownloadData(urlReportString), @"application/vnd.ms-excel", String.Format(@"Отчёт по переченю №{0}.xls", selectKrt.NKRT));
            }
=======
        //    if(selectKrt != null) {
        //        const string defaultParameters = @"rs:Format=Excel";
        //        string filterParameters = @"nkrt=" + selectKrt.NKRT + @"&y=" + reportYear;
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c

        //        string urlReportString = String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName,
        //            folderName, reportName, defaultParameters, filterParameters);

        //        //WebClient client = new WebClient { UseDefaultCredentials = true };
        //        /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
        //        WebClient client = new WebClient {
        //            Credentials =
        //                new CredentialCache
        //                {
        //                    {
        //                        new Uri("http://db2"),
        //                        @"ntlm",
        //                        new NetworkCredential(@"CPN", @"1111", @"LAN")
        //                    }
        //                }
        //        };

        //        var returnFile = File(client.DownloadData(urlReportString), @"application/vnd.ms-excel",
        //            String.Format(@"Бухгалтерский отчёт по переченю №{0}.xls", selectKrt.NKRT));

        //        //Add cookie for detect donload file on client
        //        Response.Cookies.Clear();
        //        Response.AppendCookie(new HttpCookie("SSRSfileDownloadToken", "true"));

        //        return returnFile;
        //    }

        //    return new EmptyResult();
        //}

        /// <summary>
        /// General method for donwload files
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="numberScroll"></param>
        /// <param name="reportYear"></param>
        /// <returns></returns>
<<<<<<< HEAD
        [HttpGet]
        public ActionResult BookkeeperReport(long? numberKrt) {
            const string serverName = @"desktop-lho63th";//@"DB2";
=======
        //[ChildActionOnly]
        //[FileDownloadCompleteFilter]
        public ActionResult Reports(string reportName, int? numberScroll, int? reportYear) {
            const string serverName = @"DB2";
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
            const string folderName = @"Orders";
                                                                     
            //link to SSRS buil-in repors
            if(numberScroll == null || reportYear == null) {
                string urlReportString = string.Format(@"http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}",
                    serverName, folderName,reportName,@"rs:Command=Render");

                return View("Reports", (object)urlReportString);
            }

<<<<<<< HEAD
            if (selectKrt != null) {
=======
            //check exists
            if(_bussinesEngage.GetTable<krt_Naftan>(x => x.NKRT == numberScroll && x.DTBUHOTCHET.Year == reportYear) != null) {
>>>>>>> 62abf0d42ba702bde9c78f340bf491e28d7f375c
                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = @"nkrt=" + numberScroll + @"&year=" + reportYear;

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
                    ? String.Format(@"Бухгалтерский отчёт по переченю №{0}.xls", numberScroll)
                    : String.Format(@"Отчёт о ошибках по переченю №{0}.xls", numberScroll));

                var returnFile = File(client.DownloadData(urlReportString), @"application/vnd.ms-excel", nameFile);

                //For js spinner and complete donwload callback
                Response.Cookies.Clear();
                Response.AppendCookie(new HttpCookie("SSRSfileDownloadToken", "true"));
                return returnFile;
            }
            TempData[@"message"] = String.Format(@"Невозможно вывести отчёт. Ошибка! Возможно не указан перечень");

            return RedirectToAction("Index", "Scroll");
        }
    }
}
