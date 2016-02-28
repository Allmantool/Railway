using System.Web.Mvc;
using System.Web.Routing;
using NaftanRailway.Domain.BusinessModels;

namespace NaftanRailway.WebUI {
    public static class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            /*By default, the routing system checks to see if a URL matches a disk file before evaluating the application’s routes
             If there is a match between the requested URL and a disk on the file, then the disk file is served and the routes
            defined by the application are never used. This behavior can be reversed so that the routes are evaluated before
            disk files are checked by setting the RouteExistingFiles property of the RouteCollection to true
             * On IIS server config
             * <add name="UrlRoutingModule-4.0" type="System.Web.Routing.UrlRoutingModule" preCondition="" />
             */
            routes.RouteExistingFiles = true;

            /*make the routing system less inclusive and prevent URLs from being evaluated against routes*/
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "fonts" });
            routes.IgnoreRoute("{folder}/{*pathInfo}", new { folder = "Content" });

            routes.MapRoute(
                name: "Path_Full",
                url: "{operationCategory}/Page{page}/Period{reportPeriod}/ShippingFilter{templateNumber}",
                defaults: new { controller = "Ceh18", action = "Index" },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" },
                constraints: new { page = @"\d+" });

            routes.MapRoute(
                name: "Period",
                url: "{operationCategory}/Page{page}/Period{reportPeriod}",
                defaults: new { controller = "Ceh18", action = "Index" },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" },
                constraints: new { page = @"\d+" });

            routes.MapRoute(
                name: "TypeOperation_Page",
                url: "{operationCategory}/Page{page}",
                defaults: new { controller = "Ceh18", action = "Index" },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" },
                constraints: new { page = @"\d+" });

            routes.MapRoute(
                name: "EditStorage",
                url: "PreReport/{action}/Shipping{id}",
                defaults: new { controller = "Storage", action = "Index" },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" });

            routes.MapRoute(
                name: "tempStorage",
                url: "PreReport/{action}",
                defaults: new { controller = "Storage", action = "Index" },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" });

            routes.MapRoute(
                name: "Report",
                url: "Report/{action}",
                defaults: new { controller = "Report", action = "Index" },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" });

            routes.MapRoute(
                name: "TypeOperation",
                url: "{operationCategory}",
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" },
                defaults: new { controller = "Ceh18", action = "Index", page = 1 });

            routes.MapRoute(
                name: "PagingLink",
                url: "Page{page}",
                defaults: new { controller = "Ceh18", action = "Index", operationCategory = EnumOperationType.All },
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" },
                constraints: new { page = @"\d+" });

            routes.MapRoute(
               name: "RailWayBasic",
               url: "{action}",
               defaults: new { controller = "Ceh18", action = "Index", operationCategory = EnumOperationType.All, page = 1 },
               namespaces: new[] { "NaftanRailway.WebUI.Controllers" });

            routes.MapRoute(
                name: "for_action_helper",
                url: "{controller}/{action}",
                namespaces: new[] { "NaftanRailway.WebUI.Controllers" });

            //routes.MapRoute(
            //    name:"Default",
            //    url:"{controller}/{action}/{id}",
            //    defaults:new { controller = "Home",action = "Index",id = UrlParameter.Optional }
            //);
        }
    }
}
