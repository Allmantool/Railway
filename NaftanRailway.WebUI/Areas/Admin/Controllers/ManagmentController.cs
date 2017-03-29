using NaftanRailway.WebUI.Controllers;
using System.Web.Mvc;
using System.Web.SessionState;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers {
    //[AuthorizeAD(Groups = "Rail_Developers")]
    [SessionState(SessionStateBehavior.Disabled)]
    public class ManagmentController : BaseController {
        public ActionResult ADStructure() {

            return View(model: CurrentADUser);
        }
    }
}
