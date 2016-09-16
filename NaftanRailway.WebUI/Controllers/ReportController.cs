using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using NaftanRailway.Domain.BusinessModels.SessionLogic;

namespace NaftanRailway.WebUI.Controllers {
    //[Authorize]
    public class ReportController : Controller {
        /// <summary>
        /// Render SSRS report (This method apply if you don't have Report Server enviroment
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(SessionStorage storage, string id = "PDF") {
            if (!storage.Lines.Any()) {
                ModelState.AddModelError("", @"Вы не выбрали ни одного номера отправки!");
            }

            if (ModelState.IsValid) {
                //save in db
                //_sessionRepository.AddPreReportData(storage);

                LocalReport lr = new LocalReport();

                string path = Path.Combine(Server.MapPath("~/Reports"), "General.rdlc");

                if (System.IO.File.Exists(path)) {
                    lr.ReportPath = path;
                } else {
                    ModelState.AddModelError("Path", @"Not exist report");
                    //redirect to storage index
                    return View("Index");
                }

                //ReportDataSource dc = new ReportDataSource("ReportDataSource", storage.ToReport());

                //lr.DataSources.Add(dc);

                string reportType = id;
                string mimeType;
                string encoding;
                string fileNameExtension;

                //orientarion depence on parity width and height (if table more then width => 2 page render)
                string diviceInfo =
                    "<DeviceInfo>" +
                    "  <OutputFormat>" + id + "</OutputFormat>" +
                    "  <PageWidth>12in</PageWidth>" +
                    "  <PageHeight>8in</PageHeight>" +
                    "  <MarginTop>0.5in</MarginTop>" +
                    "  <MarginLeft>0.5in</MarginLeft>" +
                    "  <MarginRight>0.5in</MarginRight>" +
                    "  <MarginBottom>0.3in</MarginBottom>" +
                    "</DeviceInfo>";

                Warning[] warnings;
                string[] streams;
                byte[] renderBytes;

                renderBytes = lr.Render(reportType, diviceInfo, out mimeType, out encoding, out fileNameExtension,
                    out streams, out warnings);

                return File(renderBytes, mimeType);
            } else {
                //Redisplay (if have some error)
                return View("Index");
            }
        }

        /// <summary>
        /// Custom binding reverse month and year for datetime type (changing uculture prop don't help)
        /// problime on iis culture and  SSRS
        /// Why argument type not date?
        /// When looking for the value to parse, the framework looks in a specific order namely:
        ///     RouteData (not shown above)
        ///     URI query string
        ///      Request form
        /// Only the last of these will be culture aware however. There is a very good reason for this, from a localization perspective. 
        /// Imagine that I have written a web application showing airline flight information that I publish online. 
        /// I look up flights on a certain date by clicking on a link for that day (perhaps something like http://www.melsflighttimes.com/Flights/2008-11-21),
        ///  and then want to email that link to my colleague in the US. The only way that we could guarantee that we will both be looking at the same page of data is 
        /// if the InvariantCulture is used. By contrast, if I'm using a form to book my flight, everything is happening in a tight cycle. The data can respect the CurrentCulture 
        /// when it is written to the form, and so needs to respect it when coming back from the form.
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="reportPeriod"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Guild18(string reportName, string reportPeriod) {
                const string serverName = @"db2";
                const string folderName = @"Orders";

                var period = DateTime.ParseExact(reportPeriod,"dd-MM-yyyy", new CultureInfo("ru", true));
                const string defaultParameters = @"rs:Format=Excel";
                string filterParameters = @"reportPeriod=" + period.ToString("MM.dd.yyyy");

                //http://desktop-lho63th/ReportServer?/Orders/krt_Naftan_Guild18Report&rs:Format=Excel&reportPeriod=01-01-2016
                string urlReportString = string.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName, folderName, reportName, defaultParameters, filterParameters);

                //WebClient client = new WebClient { UseDefaultCredentials = true };
                /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
                WebClient client = new WebClient {
                    Credentials = new CredentialCache { { new Uri("http://db2"), @"ntlm", new NetworkCredential(@"CPN", @"1111", @"LAN") } }
                };

                string nameFile = string.Format(@"Отчёт {0} за {1} {2}г.xls", reportName, period.ToString("MMMM"), period.Year);

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
    }
}