using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NaftanRailway.WebUI.Infrastructure.ModelBinders;
using NaftanRailway.BLL.Services;
using NaftanRailway.BLL.Services.Mapping;
using System.IO;
using NaftanRailway.WebUI.Infrastructure;

namespace NaftanRailway.WebUI {
    public class MvcApplication : HttpApplication {
        /// <summary>
        /// Also we have ability set different type of configuration throughout different bootstraper class (convention => name may be different)
        /// </summary>
        protected void Application_Start() {
            //Attribute Routing in MVC5
            //routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            //WebApiConfig.Register(GlobalConfiguration.Configuration);

            // This method call registers all filters
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            /* Work only with razor View Engine
             * {0} represents the name of the view.
             * {1} represents the name of the controller.
             * {2} represents the name of the area.
             */
            ViewEngines.Engines.Clear();
            /*Avoid searched each view instead in .cshtml files*/
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

            //https://github.com/doceandyou/Log4Net
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));

            /* Initialize simpleMembership
            * Install-Package Microsoft.AspNet.WebHelpers
            * Install-Package Microsoft.AspNet.WebPages.Data
            * (stop: required dependencies to Microsoft.AspNet.Razor > 3.0 => .Net 4.5
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

            //users online
            Application.Lock();
            //AppStateHelper.Set(AppStateKeys.ONLINE, 0);
            Application.Add("TotalOnlineUsers", 0);
            Application.UnLock();
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
            if (Application["TotalOnlineUsers"] != null) {
                Application.Lock();
                Application["TotalOnlineUsers"] = (int)Application["TotalOnlineUsers"] + 1;
                Application.UnLock();
            }
        }

        protected void Session_End(object sender, EventArgs e) {
            // event is raised when a session is abandoned or expires
            if (Application["TotalOnlineUsers"] != null) {
                Application.Lock();
                Application["TotalOnlineUsers"] = (int)Application["TotalOnlineUsers"] - 1;
                Application.UnLock();
            }
        }
    }
}