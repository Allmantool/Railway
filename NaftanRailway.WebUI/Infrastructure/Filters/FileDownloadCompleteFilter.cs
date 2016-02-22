using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    /// <summary>
    /// Defined then file complete download
    /// </summary>
    public class FileDownloadCompleteFilter : ActionFilterAttribute {
        public override void OnResultExecuted(ResultExecutedContext filterContext) {
            base.OnResultExecuted(filterContext);
        }
    }
}