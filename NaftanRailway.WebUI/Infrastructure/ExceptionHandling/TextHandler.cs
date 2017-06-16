using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    public class TextHandler : ExceptionHandler {
        public override void Handle(ExceptionHandlerContext context) {
            context.Result = new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.PaymentRequired));

            base.Handle(context);
        }
    }
}