using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
    public class HttpErrorController : Controller {
        public ActionResult NotFound() {
            //return HttpNotFound();
            //throw new HttpException(404, "Not found");
            Response.StatusCode = 404;
            
            return View();
        }

        public ActionResult Forbidden() {
            return RedirectToAction("NotFound");
        }

        public ActionResult ServerCrash()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}
