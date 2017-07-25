using log4net;
using NaftanRailway.BLL.Abstract;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers.api {
    public class APIScrollController : ApiController {

        private readonly INomenclatureModule _bussinesEngage;

        public APIScrollController(INomenclatureModule bussinesEngage, ILog log) {
            _bussinesEngage = bussinesEngage;
        }

        public HttpResponseMessage GetScroll() {
            var result = true;

            var msgResponse = result ? new HttpResponseMessage(HttpStatusCode.NotFound) : new HttpResponseMessage(HttpStatusCode.OK);

            return msgResponse;
        }

        /// <summary>
        /// Get filters DTO for advance searching
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult AddvanceFilter() {
            var result = true;

            return result ? (IHttpActionResult)BadRequest("No Product Found") : Ok(result);
        }
    }
}