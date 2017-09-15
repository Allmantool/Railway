using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    /// <summary>
    /// When to Use
    //Exception loggers are the solution to seeing all unhandled exception caught by Web API.
    //Exception handlers are the solution for customizing all possible responses to unhandled exceptions caught by Web API.
    //Exception filters are the easiest solution for processing the subset unhandled exceptions related to a specific action or controller.
    /// </summary>
    public class TextHandler : ExceptionHandler {
        public override void Handle(ExceptionHandlerContext context) {
            context.Result = new ResponseMessageResult(new HttpResponseMessage(HttpStatusCode.PaymentRequired));

            base.Handle(context);
        }
    }
}