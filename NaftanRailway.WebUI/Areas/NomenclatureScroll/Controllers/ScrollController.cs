using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
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
                return PartialView("_items", _bussinesEngage.GetTable<krt_Naftan>()
                    .OrderByDescending(x => x.KEYKRT).Skip(page * initialSizeItem).Take(initialSizeItem));
            }

            return View(new IndexModelView() {
                ListKrtNaftan = _bussinesEngage.GetTable<krt_Naftan>()
                    .OrderByDescending(x => x.KEYKRT).Skip(page * initialSizeItem).Take(initialSizeItem),
                ReportPeriod = DateTime.Now
            });
        }
        /// <summary>
        /// Add scroll in krt_Naftan_ORC_Sopod and download report error
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(IndexModelView model) {
            long numberKeykrt = model.ListKrtNaftan.First().KEYKRT;
            var selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT == numberKeykrt).FirstOrDefault();

            if(ModelState.IsValid && model.ReportPeriod != null && selectKrt != null &&
                _bussinesEngage.AddKrtNaftan(model.ReportPeriod.Value, selectKrt.KEYKRT)) {
                TempData["message"] = String.Format(@"Успешно добавлен перечень № {0}.", selectKrt.NKRT);
                return RedirectToAction("ErrorReport", "Scroll",new RouteValueDictionary() { {"numberKrt",numberKeykrt},{"reportYear",selectKrt.DTBUHOTCHET.Year}});
            }
 
            TempData["message"] = String.Format(@"Ошибка добавления перечень № {0}.Вероятно, он уже добавлен", selectKrt.KEYKRT);
            
            return RedirectToAction("Index", "Scroll");
        }
        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult ScrollsDetails() {
            const byte initialSizeItem = 27;
            return View(_bussinesEngage.GetTable<krt_Naftan_orc_sapod>().Take(initialSizeItem));
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
                reportName = selectKrt.SignAdjustment_list ? @"orc-bch_corrections" : @"orc-bch_compare_new";
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

                return File(client.DownloadData(urlReportString), @"application/vnd.ms-excel",String.Format(@"Отчёт по переченю №{0}",selectKrt.NKRT));

            }

            TempData[@"message"] = String.Format(@"Невозможно вывести отчёт. Ошибка!");
            return RedirectToAction("Index", "Scroll");
        }
    }
}
