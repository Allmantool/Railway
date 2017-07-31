using log4net;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.POCO;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers.api {
    public class APIScrollController : ApiController {

        private readonly INomenclatureModule _bussinesEngage;

        public APIScrollController(INomenclatureModule bussinesEngage, ILog log) {
            _bussinesEngage = bussinesEngage;
        }

        //public HttpResponseMessage GetScroll() {
        //    var result = true;

        //    var msgResponse = result ? new HttpResponseMessage(HttpStatusCode.NotFound) : new HttpResponseMessage(HttpStatusCode.OK);

        //    return msgResponse;
        //}

        /// <summary>
        /// Get filters DTO for advance searching
        /// </summary>
        /// <returns></returns>
        //[Route("customers/{customerId}/orders")]
        [HttpPost]
        [ResponseType(typeof(CheckListFilter))]
        public IHttpActionResult GetAddvanceFilter() {

            var result = (IList)_bussinesEngage.initGlobalSearchFilters();

            //var response = new HttpResponseMessage();
            //response.Headers.Add("ContentType", "application/json");

            return result.Count < 0 ? (IHttpActionResult)BadRequest("No Product Found") : Ok(result);
        }
    }
}