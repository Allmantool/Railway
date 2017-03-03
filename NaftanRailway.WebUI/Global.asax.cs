using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NaftanRailway.WebUI.Infrastructure.Binders;
using NaftanRailway.WebUI.ViewModels;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.Services.Mapping;

namespace NaftanRailway.WebUI {
    public class MvcApplication : HttpApplication {
        /// <summary>
        /// Also we hava ability set diffrent type of configuration throughtout diffrent bootstraper class (convention => name may be diffrent)
        /// </summary>
        protected void Application_Start() {
            //Attribute Routing in MVC5
            //routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            //GlobalConfiguration.Configure(WebApiConfig.Register);
            WebApiConfig.Register(GlobalConfiguration.Configuration);

            // This method call registers all filters 
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            /* Work only with razor View Engine
             * {0} represents the name of the view.
             * {1} represents the name of the controller.
             * {2} represents the name of the area.
             */
            ViewEngines.Engines.Clear();
            /*Avoid seached each view instead in .cshtml files*/
            ViewEngines.Engines.Add(new RazorViewEngine() {
                ViewLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml" },
                PartialViewLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml" },
                MasterLocationFormats = new[] { "~/Views/{1}/{0}.cshtml", "~/Views/Shared/{0}.cshtml" },
                AreaViewLocationFormats = new[] { "~/Areas/{2}/Views/{1}/{0}.cshtml", "~/Areas/{2}/Views/Shared/{0}.cshtml" },
                AreaMasterLocationFormats = new[] { "~/Areas/{2}/Views/{1}/{0}.cshtml", "~/Areas/{2}/Views/Shared/{0}.cshtml" },
                AreaPartialViewLocationFormats = new[] { "~/Areas/{2}/Views/{1}/{0}.cshtml", "~/Areas/{2}/Views/Shared/{0}.cshtml" }
            });

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //registration custom binding providers with links to vm
            ModelBinders.Binders.Add(typeof(SessionStorage), new StorageTableModelBinder());
            //ModelBinders.Binders.Add(typeof(InputMenuViewModel), new InputMenuModelBinder());

            //MVC4 Quick Tip #3–Removing the XML Formatter from ASP.Net Web API
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            //Configure AutoMapper
            AutoMapperBLLConfiguration.Configure();

            /* Inizialize simpleMembership
            * Install-Package Microsoft.AspNet.WebHelpers
            * Install-Package Microsoft.AspNet.WebPages.Data
            * (stop: requered dependesies to Microsoft.AspNet.Razor > 3.0 => .Net 4.5
            * WebSecurity.InitializeDatabaseConnection("SecurityConnection", "UserProfile", "UserId", "UserName", true);
            */

            /*Custom value provider (order sense)
             * (First)
             * ValueProviderFactories.Factories.Insert(0,new CustomValueProviderFactory());
             * (End)
             * ValueProviderFactories.Factories.Add(new CustomValueProviderFactory());
             */

            /*Controller Builder
             * ControllerBuilder.Current.DefaultNamespaces.Add("DefaultNamespace");
             */

            //connected modules
            //var info = Modules;
        }

        /// <summary>
        /// Handling as a fallback for any unexpected and unhandled errors
        /// The route errors (404) is not mapped to ASP.NET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e) {
            Exception exception = Server.GetLastError();
            if (exception is HttpUnhandledException) {
                exception = exception.InnerException;
            }

            // log exception message using   
            var thisRequest = HttpContext.Current.Request;

            if (exception != null && thisRequest.IsLocal) {
                System.Diagnostics.Debug.WriteLine(exception.Message);

                //Response.Redirect("~Views/Shared/Errors.cshtml");
                //Server.ClearError();
                //System.Diagnostics.Debugger.Break();
            }
        }

        protected void Session_Start(object sender, EventArgs e) {
            // event is raised each time a new session is created     
        }

        protected void Session_End(object sender, EventArgs e) {
            // event is raised when a session is abandoned or expires
        }
    }
}