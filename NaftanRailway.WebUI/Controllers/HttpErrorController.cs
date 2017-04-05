using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;
using System;
using System.Diagnostics;

namespace NaftanRailway.WebUI.Controllers {
    [AllowAnonymous]
    public class HttpErrorController : BaseController {
        public ActionResult NotFound() {
            //throw new HttpException(404, "Not found");
            //var modules = HttpContext.ApplicationInstance.Modules;

            Response.StatusCode = 404;
            return View();
        }

        public ActionResult Forbidden() {
            return RedirectToAction("NotFound");
        }

        public ActionResult ServerCrash() {
            Response.StatusCode = 500;

            //var result = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "#";

            return View();
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
                FromEmail = CurrentADUser.EmailAddress,
                FromName = CurrentADUser.FullName,
            };

            return View(model: sender);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMail(EmailFormViewModel model) {
            if (ModelState.IsValid) {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";

                using (var message = new MailMessage())
                using (var smtp = new SmtpClient() { UseDefaultCredentials = true }) {
                    message.To.Add(new MailAddress("P.Chizhikov@naftan.by"));  // replace with valid value
                    message.From = new MailAddress(CurrentADUser.EmailAddress);  // replace with valid value
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

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}