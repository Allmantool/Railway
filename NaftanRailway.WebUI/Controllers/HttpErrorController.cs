using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Controllers {
    [AllowAnonymous]
    public class HttpErrorController : BaseController {
        public ActionResult NotFound() {
            //return HttpNotFound();
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
            Response.StatusCode = 401;

            //Response.ClearHeaders();
            //Response.AddHeader("WWW-Authenticate", "Basic");
            //Response.Headers.Remove("WWW-Authenticate");
            //if (!Request.IsLocal && Request.IsAuthenticated) {
            //    return View(model: CurrentADUser.Name);
            //}
            var sender = new EmailFormViewModel() {
                FromEmail = CurrentADUser.EmailAddress,
                FromName = CurrentADUser.Name,
            };

            return View(model: sender);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendMail(EmailFormViewModel model) {
            if (ModelState.IsValid) {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress("P.Chizhikov@naftan.by"));  // replace with valid value
                message.From = new MailAddress(CurrentADUser.EmailAddress);  // replace with valid value
                message.Subject = "Proposal to add right";
                message.Body = string.Format(body, model.FromName, model.FromEmail, model.Message);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient() { UseDefaultCredentials = true }) {
                    //var credential = new NetworkCredential {
                    //    UserName = "user@outlook.com",  // replace with valid value
                    //    Password = "password"  // replace with valid value
                    //};
                    //smtp.Credentials = credential;
                    smtp.Host = "naftan.by";
                    smtp.Port = 25;
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(message);

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            return View(model);
        }
    }
}