using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    /// <summary>
    /// We can use exception filter to catch unhandled exceptions on action / controllers.
    /// </summary>
    public class WebApi2CustomExceptionFilter : ExceptionFilterAttribute {
        public override void OnException(HttpActionExecutedContext actionExecutedContext) {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null) {
                exceptionMessage = actionExecutedContext.Exception.Message;
            } else {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }
            //We can log this exception message to the file or database.  
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                Content = new StringContent("An unhandled exception was thrown by service (Web API 2)."),
                ReasonPhrase = "Internal Server Error. Please Contact your Administrator or developer."
            };

            actionExecutedContext.Response = response;
        }
    }
}