using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;
using System;
using System.Diagnostics;
using log4net;

namespace NaftanRailway.WebUI.Controllers {
    [AllowAnonymous]
    public class HttpErrorController : BaseController {
        public HttpErrorController(ILog logger) : base(logger) {
        }

        public ActionResult NotFound() {
            //throw new HttpException(404, "Not found");
            //var modules = HttpContext.ApplicationInstance.Modules;

            this.Response.StatusCode = 404;
            return this.View();
        }

        public ActionResult Forbidden() {
            return this.RedirectToAction("NotFound");
        }

        public ActionResult ServerCrash() {
            this.Response.StatusCode = 500;

            //var result = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "#";

            return this.View();
        }

        public ActionResult NotAuthorized() {
            //show or not windows auth pop up
            //Response.StatusCode = 401;

            //Response.ClearHeaders();
            //Response.AddHeader("WWW-Authenticate", "Basic");
            //Response.Headers.Remove("WWW-Authenticate");

            //if (!Request.IsLocal && Request.IsAuthenticated) {
            //    return View(model: CurrentADUser.Name);
            //}
            var sender = new EmailFormViewModel() {
                FromEmail = this.CurrentADUser.EmailAddress,
                FromName = this.CurrentADUser.FullName,
            };

            return this.View(model: sender);
        }

        //[HttpPost]
        public FileContentResult DiagnosticLog() {
            return this.GetLog();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMail(EmailFormViewModel model) {

            if (this.ModelState.IsValid) {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

                using (var message = new MailMessage())
                using (var smtp = new SmtpClient() { UseDefaultCredentials = true }) {
                    message.To.Add(new MailAddress("P.Chizhikov@naftan.by"));  // replace with valid value
                    message.From = new MailAddress(this.CurrentADUser.EmailAddress);  // replace with valid value
                    message.Subject = "Proposal to add right";
                    message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                    message.IsBodyHtml = true;

                    smtp.Host = "naftan.by";
                    smtp.Port = 25;
                    smtp.EnableSsl = false;

                    try {
                        await smtp.SendMailAsync(message);
                    } catch (Exception ex) {
                        Debug.Write(ex.Message);
                    }

                    return this.Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return this.Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}