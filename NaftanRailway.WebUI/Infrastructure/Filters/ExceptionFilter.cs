using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    /// <summary>
    /// Custom filter for handing exceptions
    /// *Another kind of filter (authorization, action, or result filter)
    /// *the aciton method itself
    /// *when the action result is executed
    /// </summary>
    public class ExceptionFilterAttribute : FilterAttribute, IExceptionFilter {
        /// <summary>
        /// Called when an unhandled exception arises
        /// </summary>
        /// <param name="filterContext">Direved from ControllerContext
        /// Useful ControllerContext prop:
        ///     Controller => Return the controller object for this request
        ///     HttpContext => Provides accesse to datails of a request and access to the response
        ///     IsChildAction 
        ///     RequestContext => Provides access to the HttpContext and the routing data, both of which are avaible through other properties
        ///     RouteData => Returns the routing data for this request
        /// Useful ExceptionContext prop:
        ///     ActionDescriptor => Provides details of the action method
        ///     Result => The result for action method; a filter can cancel request by setting this property to a non-null value
        ///     Exception => the unhandled exception
        ///     ExceptionHandled => Returns true if another filter has marked the exception as handled
        /// </param>
        public void OnException(ExceptionContext filterContext) {
            if (!filterContext.ExceptionHandled) {
                //filterContext.Result = new RedirectResult("~/Views/Shared/Errors.cshtml");
                filterContext.Result = new ViewResult() { ViewName = "Errors", ViewData = new ViewDataDictionary<ExceptionContext>(filterContext) };
                //mark exception as handled (other filters not doing attepting work)
                filterContext.ExceptionHandled = true;
            }
        }
    }
}