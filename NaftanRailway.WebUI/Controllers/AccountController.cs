using System.Web.Mvc;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.Domain.BusinessModels.AuthorizationLogic;
using NaftanRailway.WebUI.ViewModels;


namespace NaftanRailway.WebUI.Controllers {
    public class AccountController : Controller {
        private readonly IAuthorizationEngage _engage;

        public AccountController(IAuthorizationEngage engage) {
            this._engage = engage;
        }
    }
}
