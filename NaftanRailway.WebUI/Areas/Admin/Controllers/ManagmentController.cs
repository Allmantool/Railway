using NaftanRailway.WebUI.Controllers;
using System.Web.Mvc;
using System.Web.SessionState;
using log4net;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers {
    //[AuthorizeAD(Groups = "Rail_Developers")]
    [SessionState(SessionStateBehavior.Disabled)]
    public class ManagmentController : BaseController {
        public ManagmentController(ILog logger) : base(logger) {}

        public ActionResult ADStructure() {

            return this.View(model: this.CurrentADUser);
        }
    }
}