using NaftanRailway.BLL.Abstract;
using System.Web.Http;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers.api {
    [AllowAnonymous]
    public class AdminController : ApiController {
        private readonly IAuthorizationEngage _authLogic;

        public AdminController(IAuthorizationEngage authLogic) {
            _authLogic = authLogic;
        }

        public IHttpActionResult GetAdminPrincipal() {
            var result = _authLogic.AdminPrincipal(User.Identity.Name);

            return Ok(result);
        }

        public IHttpActionResult GetGroupMembers(string id) {
            var result = _authLogic.GetMembers(id);

            if (result == null) return NotFound();

            return Ok(result);
        }
    }
}