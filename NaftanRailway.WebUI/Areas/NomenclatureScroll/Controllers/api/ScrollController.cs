using log4net;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.POCO;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

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
        [CacheOutput(ClientTimeSpan = 5000, ServerTimeSpan = 5000)]
        public IHttpActionResult GetAddvanceFilter() {

            var result = (IList<CheckListFilter>)_bussinesEngage.initGlobalSearchFilters();

            //var response = new HttpResponseMessage();
            //response.Headers.Add("ContentType", "application/json");

            return result.Count < 0 ? (IHttpActionResult)BadRequest("No Product Found") : Ok(result);
        }
    }
}