using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using NaftanRailway.BLL.DTO.General;
using System.Globalization;

namespace NaftanRailway.UnitTests.General {
    [TestClass]
    public class ReportTests {
        [TestMethod]
        public void General() {
            const string serverName = @"db2";
            const string folderName = @"Orders";
            const string reportName = @"krt_Naftan_Guild18Report";
            DateTime reportPeriod = new DateTime(2017, 4, 1);

            var brInfo = new BrowserInfoDTO { Name = "Test", Version = "Test" };

            var period = reportPeriod; //DateTime.ParseExact(reportPeriod, "dd-MM-yyyy", new CultureInfo("ru", true));
            const string defaultParameters = @"rs:Format=Excel";
            string filterParameters = @"reportPeriod=" + period.ToString("MM.01.yyyy");

            //http://db2/ReportServer?/Orders/krt_Naftan_Guild18Report&rs:Format=Excel&reportPeriod=01-01-2016
            string urlReportString = string.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName, folderName, reportName, defaultParameters, filterParameters);

            WebClient client = new WebClient { UseDefaultCredentials = true };
            /*System administrator can't resolve problem with old report (Kerberos don't work on domain folder)*/
            //WebClient client = new WebClient {
            //    Credentials = new CredentialCache { { new Uri("http://db2"), @"ntlm", new NetworkCredential(@"CPN", @"1111", @"LAN") } }
            //};

            string nameFile = string.Format(@"Отчёт {0} за {1} {2}г.xls", reportName, period.ToString("MMMM"), period.Year);

            //Changing "attach;" to "inline;" will cause the file to open in the browser instead of the browser prompting to save the file.
            //encode the filename parameter of Content-Disposition header in HTTP (for support different browser)
            string contentDisposition;
            if (brInfo.Name == "IE" &&
                (brInfo.Version == "7.0" || brInfo.Version == "8.0"))
                contentDisposition = "attachment; filename=" + Uri.EscapeDataString(nameFile);
            else if (brInfo.Name == "Safari")
                contentDisposition = "attachment; filename=" + nameFile;
            else
                contentDisposition = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(nameFile);

            //name file (with encoding)
            client.Headers.Add("Content-Disposition", contentDisposition);

            byte[] data = { };
            var exc = String.Empty;

            try {
                data = client.DownloadData(urlReportString);
            } catch (Exception exMsg) {
                exc = exMsg.Message;
            }

            if (data.Length > 0) {
                //For js spinner and complete download callback
                Assert.IsTrue(data.Length > 0);
            }

            Assert.IsTrue(exc != String.Empty);
        }
    }
}
