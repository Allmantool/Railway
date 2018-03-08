using log4net;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Nomenclature;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers.api {
    [RoutePrefix("api")]
    public class ApiScrollController : ApiController {
        private readonly INomenclatureModule _bussinesEngage;

        public ApiScrollController(INomenclatureModule bussinesEngage, ILog log) {
            this._bussinesEngage = bussinesEngage;
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
        [HttpGet, Route("DocTree/{typeDoc:int:min(0)?}/{rootKey?}")]
        public IHttpActionResult GetExpandTree(int? typeDoc = null, string rootKey = null) {
            //if (rootKey.Length == 0) {
            //    var message = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            //        Content = new StringContent("We cannot use empty rootKey")
            //    };
            //    throw new HttpResponseException(message);
            //}

            //var result = (IList<CheckListFilter>)_bussinesEngage.initGlobalSearchFilters();
            var tree = (typeDoc == null) ? this._bussinesEngage.GetTreeStructure(rootKey: rootKey) : this._bussinesEngage.GetTreeStructure(typeDoc.Value, rootKey);

            var response = new HttpResponseMessage();
            response.Headers.Add("ContentType", "application/json");

            //return Ok($"TypeDoc: {typeDoc ?? 0}, rootKey: {rootKey?? " empty"}");
            return tree.Count < 0 ? (IHttpActionResult)this.BadRequest("No Nodes Found") : this.Ok(tree);
        }
    }
}