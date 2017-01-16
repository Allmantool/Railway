using System.Diagnostics;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    /// <summary>
    /// To overcome limitations ASP.MVC provide functionality to define custom exception filter by extending HandleErrorAttribute
    /// Custom filter for handing exceptions
    /// *Another kind of filter (authorization, action, or result filter)
    /// *the aciton method itself
    /// *when the action result is executed
    /// </summary>
    public class ExceptionFilterAttribute : HandleErrorAttribute {// FilterAttribute, IExceptionFilter
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
        public override void OnException(ExceptionContext filterContext) {
            base.OnException(filterContext);
            //First Check if Exception all ready Handle Or Check Is Custom error Handle is enable
            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (filterContext.ExceptionHandled  /*|| !filterContext.HttpContext.IsCustomErrorEnabled*/) {
                return;
            }

            var statusCode = (int)HttpStatusCode.InternalServerError;
            if (filterContext.Exception is HttpException) {
                statusCode = new HttpException(null, filterContext.Exception).GetHttpCode();
            }

            //prepare error info object
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            var errormodel = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            // if the request is AJAX return JSON else view. At Ajax request time, If any exception occurred then its will return error view , which is not a good things
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
                filterContext.Result = new JsonResult {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new {
                        error = true,
                        message = filterContext.Exception.Message,
                        errorObj = JsonConvert.SerializeObject(errormodel, new JsonSerializerSettings {
                            Formatting = Formatting.Indented,
                            //TypeNameHandling = TypeNameHandling.Objects,
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        }),
                        source = filterContext.Exception.Source
                    },
                    ContentEncoding = System.Text.Encoding.UTF8,
                    ContentType = "application/json"
                };
            } else {
                var modules = HttpContext.Current.ApplicationInstance.Modules;

                //exception + httpModules
                var exceptContext = new ExceptionViewModel {
                    Model = filterContext,
                    Modules = modules.AllKeys
                            .Select(x => new Tuple<string, string>(x.StartsWith("__Dynamic") ? string.Format("Dynamic: {0},{1},{2}", x.Split('_', ',')[3], x.Split('_', ',')[4], x.Split('_', ',')[5]) : x, modules[x].GetType().Name))
                            .OrderBy(x => x.Item1).ToArray()
            };

                filterContext.Result = (new ViewResult() {
                    ViewName = "Errors",
                    MasterName = "_Layout.HttpErrors",
                    //ViewData = new ViewDataDictionary(errormodel),
                    ViewData = new ViewDataDictionary<ExceptionViewModel>(exceptContext),
                    TempData = filterContext.Controller.TempData
                });
            }

            //mark exception as handled (other filters not doing attepting work)
            Debug.WriteLine(filterContext.Exception.Message);
            // Prepare the response code.  
            filterContext.ExceptionHandled = true;
            //filterContext.HttpContext.Response.Clear();

            if (!filterContext.HttpContext.Request.IsLocal) { filterContext.HttpContext.Response.StatusCode = statusCode; }

            // Certain versions of IIS will sometimes use their own error page when
            // they detect a server error. Setting this property indicates that we
            // want it to try to render ASP.NET MVC's error page instead.
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}