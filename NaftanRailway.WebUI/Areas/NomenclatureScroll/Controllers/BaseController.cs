using System.Web.Mvc;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    /// <summary>
    /// Use BaseController
    //  It is recommended to use Base Controller with our Controller and this Base Controller will inherit Controller class directly. 
    /// It provides isolation space between our Controller[InterviewController] and Controller.
    /// Using Base Controller, we can write common logic which could be shared by all Controllers.
    /// </summary>
    public class BaseController : Controller {
        //protected virtual LoginPrincipal LoggedUser {
        //    get { return HttpContext.User as LoginPrincipal; }
        //}

        //protected int GetUserId() {
        //    int userId = -1;
        //    if (Request.IsAuthenticated) {
        //        userId = LoggedUser.UserId;
        //    }
        //    return userId;
        //}

    }
}
