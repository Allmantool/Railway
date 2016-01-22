using System;
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
            var selectKrt = _bussinesEngage.GetTable<krt_Naftan>(x => x.KEYKRT==numberKeykrt).FirstOrDefault();

            if(ModelState.IsValid && model.ReportPeriod != null && selectKrt != null
                /*&& _bussinesEngage.AddKrtNaftan(model.ReportPeriod.Value, selectKrt.KEYKRT)*/) {
                const string serverName = @"DB2";
                const string folderName = @"Orders";
                string reportName = selectKrt.SignAdjustment_list ? @"orc-bch_corrections" : @"orc-bch_compare_new";
                const string defaultParameters = @"&rs:Command=Render&rs:Format=PDF";
                string filterParameters = @"&nkrt=" + selectKrt.NKRT + @"&y=" + selectKrt.DTBUHOTCHET.Year;

                string urlReportString =String.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}{4}", serverName, folderName, reportName, defaultParameters, filterParameters);

                //WebClient client = new WebClient {
                //    Credentials = CredentialCache.DefaultCredentials,
                //    UseDefaultCredentials = true
                //};
                

                //return File(client.DownloadData(urlReportString), "application/pdf");

                WebClient client = new WebClient();
                
                var cc = new CredentialCache{
                    { new Uri("http://db2"), "NTLM", new NetworkCredential("cpn", "1111", "LAN") }
                };
                client.Credentials = cc;

                return File(client.DownloadData(urlReportString), "application/pdf");

            }

            if(selectKrt != null)
                TempData["message"] = String.Format("Невозможно добавить перечень № {0}. т.к он уже добавлен", selectKrt.NKRT);

            return RedirectToAction("Index", "Scroll");
        }
        /// <summary>
        /// Return krt_Naftan_orc_sapod
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult ScrollsDetails() {
            return View(_bussinesEngage.GetTable<krt_Naftan_orc_sapod>().Take(100));
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
        public ViewResult ErrorReport(string reportName) {
            const string serverName = @"DB2";
            const string folderName = @"Orders";

            string urlReportString = String.Format("http://{0}/ReportServer/Pages/ReportViewer.aspx?/{1}/{2}&{3}",
                                        serverName, folderName,
                                        reportName,
                                        @"rs:Command=Render");

            return View((object)urlReportString);
        }
    }
}