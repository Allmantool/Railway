using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
    public class HttpErrorController : Controller {
        public ActionResult NotFound() {
            //return HttpNotFound();
            //throw new HttpException(404, "Not found");
            Response.StatusCode = 400;
            
            return View();
        }

        //public ActionResult Forbidden() {
        //    return View();
        //}
    }
}
