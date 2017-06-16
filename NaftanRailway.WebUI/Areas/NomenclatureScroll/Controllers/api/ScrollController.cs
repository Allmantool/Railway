using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers.api {
    public class APIScrollController : ApiController {
        public HttpResponseMessage GetScroll() {
            var result = true;

            var msgResponse = result ? new HttpResponseMessage(HttpStatusCode.NotFound) : new HttpResponseMessage(HttpStatusCode.OK);
            
            return msgResponse;
        }


        //web api 2
        //public IHttpActionResult GetScroll() {
        //    var result = true;

        //    return result ? (IHttpActionResult)BadRequest("No Product Found") : Ok(result);
        //}
    }
}
