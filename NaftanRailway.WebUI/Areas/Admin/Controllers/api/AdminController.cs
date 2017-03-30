using NaftanRailway.BLL.Abstract;
using System.Web.Http;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers.api {
    public class AdminController : ApiController {
        private readonly IAuthorizationEngage _authLogic;

        public AdminController(IAuthorizationEngage authLogic) {
            _authLogic = authLogic;
        }

        public IHttpActionResult GetAdminPrincipal() {
            var result = _authLogic.AdminPrincipal(User.Identity.Name);

            return Ok();
        }
    }
}
