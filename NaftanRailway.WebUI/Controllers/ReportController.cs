using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;

namespace NaftanRailway.WebUI.Controllers {
    [Authorize]
    public class ReportController :Controller {
        private readonly ISessionDbRepository _sessionRepository;

        public ReportController(ISessionDbRepository sessionRepository) {
            _sessionRepository = sessionRepository; 
        }

        /// <summary>
        /// Render SSRS report
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(SessionStorage storage,string id = "PDF") {
            if (!storage.Lines.Any()) {
                ModelState.AddModelError("",@"Вы не выбрали ни одного номера отправки!");
            }

            if (ModelState.IsValid) {
                //save in db
                //_sessionRepository.AddPreReportData(storage);

                LocalReport lr = new LocalReport();

                string path = Path.Combine(Server.MapPath("~/Reports"),"General.rdlc");

                if (System.IO.File.Exists(path)) {
                    lr.ReportPath = path;
                } else {
                    ModelState.AddModelError("Path",@"Not exist report");
                    //redirect to storage index
                    return View("Index");
                }

                ReportDataSource dc = new ReportDataSource("ReportDataSource",storage.ToReport());

                lr.DataSources.Add(dc);

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

                renderBytes = lr.Render(reportType,diviceInfo,out mimeType,out encoding,out fileNameExtension,
                    out streams,out warnings);

                return File(renderBytes,mimeType);
            } else {
                //Redisplay (if have some error)
                return View("Index");
            }
        }
    }
}
