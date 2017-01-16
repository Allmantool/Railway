using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;

namespace NaftanRailway.WebUI.Controllers {
    [AllowAnonymous]
    public class HttpErrorController : Controller {
        public ActionResult NotFound() {
            //return HttpNotFound();
            //throw new HttpException(404, "Not found");
            var modules = HttpContext.ApplicationInstance.Modules;

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
            //Response.StatusCode = 401;

            //Response.ClearHeaders();
            //Response.AddHeader("WWW-Authenticate", "Basic");
            //Response.Headers.Remove("WWW-Authenticate");
            if (!Request.IsLocal && Request.IsAuthenticated) {
                var domainName = (HttpContext.User.Identity.Name.Substring(0, 7).ToLower() == "polymir" ? "POLYMIR.NET" : "lan.naftan.by");

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName)) {
                    using (UserPrincipal adUser = UserPrincipal.FindByIdentity(context, User.Identity.Name)) {
                        if (adUser != null) {
                            return View(adUser);
                        }
                    }
                }
            }
            return View();
        }
    }
}