using NaftanRailway.BLL.Abstract;
using System.Web.Http;
using System.Web.Http.Description;
using NaftanRailway.BLL.DTO.Admin;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Configuration;

namespace NaftanRailway.WebUI.Areas.Admin.Controllers.api {
    [AllowAnonymous]
    public class AdminController : ApiController {
        private readonly IAuthorizationEngage _authLogic;

        public AdminController(IAuthorizationEngage authLogic) {
            _authLogic = authLogic;
        }

        [ResponseType(typeof(ADUserDTO))]
        public IHttpActionResult GetAdminPrincipal() {
            var result = _authLogic.AdminPrincipal(User.Identity.Name);

            return Ok(result);
        }

        //responseType for documentation page
        [ResponseType(typeof(ADUserDTO))]
        public IHttpActionResult GetGroupMembers(string id) {
            var result = _authLogic.GetMembers(id);

            if (result == null) return NotFound();

            return Ok(result);
        }

        private IHttpActionResult GetConnStrings() {
            Dictionary<string, string> configData = new Dictionary<string, string>();

            foreach (ConnectionStringSettings cs in WebConfigurationManager.ConnectionStrings) {
                configData.Add(cs.Name, string.Format(@"{0} {1}", cs.ProviderName, cs.ConnectionString));
            }

            return Ok(configData);
        }
    }
}