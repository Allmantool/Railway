using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
    [AllowAnonymous]
    public class HttpErrorController : BaseController {
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
            //if (!Request.IsLocal && Request.IsAuthenticated) {
            //    return View(model: CurrentADUser.Name);
            //}
            return View();
        }
    }
}