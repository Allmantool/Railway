using System.Web.Mvc;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    public class ConfigurationController : AsyncController {
        /// <summary>
        /// Configuration page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index() {
            return View();
        }

    }
}
