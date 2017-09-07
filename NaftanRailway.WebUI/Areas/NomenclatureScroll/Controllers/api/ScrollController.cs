using log4net;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Nomenclature;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers.api {
    //[RouteArea("Member")]
    //[RoutePrefix("member")]
    public class ApiScrollController : ApiController {
        private readonly INomenclatureModule _bussinesEngage;

        public ApiScrollController(INomenclatureModule bussinesEngage, ILog log) {
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
        [ResponseType(typeof(TreeNode))]
        [CacheOutput(ClientTimeSpan = 5000, ServerTimeSpan = 5000)]
        [Route("api/APIScroll/{typeDoc}/{rootKey}")]
        [Route("api/APIScroll")]
        [HttpGet]
        public IHttpActionResult ExpandTree(int? typeDoc = null, string rootKey = null) {
            //var result = (IList<CheckListFilter>)_bussinesEngage.initGlobalSearchFilters();
            var tree = (typeDoc == null) ? _bussinesEngage.GetTreeStructure(rootKey: rootKey) : _bussinesEngage.GetTreeStructure(typeDoc.Value, rootKey);

            var response = new HttpResponseMessage();
            response.Headers.Add("ContentType", "application/json");

            return tree.Count < 0 ? (IHttpActionResult)BadRequest("No Nodes Found") : Ok(tree);
        }
    }
}