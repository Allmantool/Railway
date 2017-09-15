using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    /// <summary>
    ///  We can use Exception Handlers to catch any type of unhandled exception application-wide.
    ///  Web API 2 provides a good alternative way to achieve global exception handling.
    ///  Web API provides "ExceptionHandler" abstract class to handle exception above said area.
    ///  * Error inside the exception filter.
    ///  * Exception related to routing.
    ///  * Error inside the Message Handlers class.
    ///  * Error in Controller Constructor.
    /// </summary>
    public class GlobalExceptionHandler : ExceptionHandler {
        public async override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken) {
            // Access Exception using context.Exception;  
            const string errorMessage = "An unexpected error occured in Web API2";
            var response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, new { Message = errorMessage });

            response.Headers.Add("X-Error", errorMessage);
            context.Result = new ResponseMessageResult(response);
        }
    }
}