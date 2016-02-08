using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Filters;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.WebUI.Infrastructure.Binders;
using NaftanRailway.WebUI.ViewModels;
using WebMatrix.WebData;

namespace NaftanRailway.WebUI {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(typeof(SessionStorage), new StorageTableModelBinder());
            ModelBinders.Binders.Add(typeof(InputMenuViewModel), new InputMenuModelBinder());

            /* Step 3: Inizialize simpleMembership db
             Install-Package Microsoft.AspNet.WebHelpers
             Install-Package Microsoft.AspNet.WebPages.Data
             (stop: requered dependesies to Microsoft.AspNet.Razor > 3.0 => .Net 4.5
             */
            WebSecurity.InitializeDatabaseConnection("SecurityConnection", "UserProfile", "UserId", "UserName", true);
        }
    }
}