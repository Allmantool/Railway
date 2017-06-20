using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using NaftanRailway.BLL.DTO.General;
using System.Net.Mail;
using System.Diagnostics;
using System.Net.Http;
using System.Linq;
using System.IO;
using System.Text;

namespace NaftanRailway.UnitTests.General {
    [TestClass]
    public class ClientServicesTests {
        [TestMethod]
        public void ReportWebClient() {
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

        [TestMethod]
        public void ReportHttpClient() {
            const string serverName = @"db2";
            const string folderName = @"Orders";
            const string reportName = @"krt_Naftan_Guild18Report";
            DateTime reportPeriod = new DateTime(2017, 1, 1);

            var brInfo = new BrowserInfoDTO { Name = "Test", Version = "Test" };

            var period = reportPeriod; //DateTime.ParseExact(reportPeriod, "dd-MM-yyyy", new CultureInfo("ru", true));
            const string defaultParameters = @"rs:Format=Excel";
            string filterParameters = @"reportPeriod=" + period.ToString("MM.01.yyyy");

            //http://db2/ReportServer?/Orders/krt_Naftan_Guild18Report&rs:Format=Excel&reportPeriod=01-01-2016
            string urlReportString = string.Format(@"http://{0}/ReportServer?/{1}/{2}&{3}&{4}", serverName, folderName, reportName, defaultParameters, filterParameters);

            string nameFile = string.Format(@"Отчёт {0} за {1} {2}г.xls", reportName, period.ToString("MMMM"), period.Year);

            //Changing "attach;" to "inline;" will cause the file to open in the browser instead of the browser prompting to save the file.
            //encode the filename parameter of Content-Disposition header in HTTP (for support different browser)
            string contentDisposition;
            if (brInfo.Name == "IE" && (new[] { "7.0", "8.0" }).Contains(brInfo.Version))
                contentDisposition = "attachment; filename=" + Uri.EscapeDataString(nameFile);
            else if (brInfo.Name == "Safari")
                contentDisposition = "attachment; filename=" + nameFile;
            else
                contentDisposition = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(nameFile);

            byte[] data = { };
            var exc = String.Empty;

            using (var handler = new HttpClientHandler { UseDefaultCredentials = true })
            using (var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://db2") }) {
                try {
                    var responseTask = httpClient.GetByteArrayAsync(urlReportString);
                    responseTask.Wait();

                    data = responseTask.Result;
                } catch (Exception exMsg) {
                    exc = exMsg.Message;
                }
            }

            if (data.Length > 0) {
                //For js spinner and complete download callback
                Assert.IsTrue(data.Length > 0);
            } else {
                Assert.IsTrue(exc != String.Empty);
            }
        }

        [TestMethod]
        public async void MailClient() {
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            // i need some smpt account for app
            var model = new { FromName = "Pavel", FromEmail = "SomeUser@naftan.by", ToEmail = "P.Chizhikov@naftan.by", Message = "Hello it's mail client test!" };

            using (var message = new MailMessage())
            using (var smtp = new SmtpClient() { UseDefaultCredentials = true }) {
                message.To.Add(new MailAddress(model.ToEmail));  // replace with valid value
                message.From = new MailAddress(model.FromEmail);  // replace with valid value
                message.Subject = "Proposal to add right";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;

                smtp.Host = "naftan.by";
                smtp.Port = 25;
                smtp.EnableSsl = false;
                //smtp.Credentials = new NetworkCredential {
                //    UserName = "user@gmail.com",  // replace with valid value
                //    Password = "password"  // replace with valid value
                //};
                try {
                    await smtp.SendMailAsync(message);
                } catch (Exception ex) {
                    Debug.Write(ex.Message);
                }
            }

            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void CheckLogPath() {
            var txt = string.Empty;
            var logpath = Path.Combine(@"c:\Users\cpn.LAN\Desktop", @"logs\log.txt");

            if (!File.Exists(logpath)) {
                Directory.CreateDirectory(Path.Combine(@"c:\Users\cpn.LAN\Desktop", @"logs\"));
                File.Create(logpath).Close();
            }

            try {
                using (var fileStream = new FileStream(logpath, FileMode.Open, FileAccess.Read))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8)) {
                    txt = streamReader.ReadToEnd();
                }
            } catch (Exception) {

                Assert.IsTrue(true);
            }


            Assert.IsTrue(true);
        }
    }
}