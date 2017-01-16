using System.Web.Http;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers {
    public class ManagmentController : ApiController {
        //public ActionResult Modules() {
        //    var modules = HttpContext.ApplicationInstance.Modules;

        //    Tuple<string, string>[] data = modules.AllKeys.Select(x => new Tuple<string, string>(x.StartsWith("__Dynamic") ? x.Split('_', ',')[3] : x, modules[x].GetType().Name))
        //    .OrderBy(x => x.Item1).ToArray();

        //    return View(data);
        //}
    }
}
