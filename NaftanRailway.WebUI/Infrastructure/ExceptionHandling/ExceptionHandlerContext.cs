using System.Web.Http;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    public class ExceptionHandlerContext {
        public ExceptionContext ExceptionContext { get; set; }
        public IHttpActionResult Result { get; set; }
    }
}