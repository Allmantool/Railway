using NaftanRailway.BLL.Abstract;
using System.Web.Http;
using NaftanRailway.BLL.DTO.Admin;
using System.Collections.Generic;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers.api {
    public class AdminController : ApiController {
        private readonly IAuthorizationEngage _authLogic;

        public AdminController(IAuthorizationEngage authLogic) {
            _authLogic = authLogic;
        }

        public ADUserDTO GetAdminPrincipal() {
            var result = _authLogic.AdminPrincipal(User.Identity.Name);

            return result;
        }

        public IEnumerable<ADUserDTO> GetGroupMembers(string Id) {
            var result = _authLogic.GetMembers(Id);

            return result;
        }
    }
}