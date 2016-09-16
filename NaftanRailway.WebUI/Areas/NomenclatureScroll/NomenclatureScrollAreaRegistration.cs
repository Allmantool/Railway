using System.Web.Mvc;
using System.Web.Routing;
//using System.Web.Mvc.Routing.Constraints; (attribute routing)

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll {
    public class NomenclatureScrollAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "NomenclatureScroll";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.Routes.IgnoreRoute("Nomenclature/{resource}.axd/{*pathInfo}",new StopRoutingHandler());
            context.MapRoute(
                name: "ReportRoutesWithParams",
                url: "{area}/{controller}/{action}/{reportName}/{numberScroll}/{reportYear}/{*catchall}",
                defaults: new { area = "Nomenclature", action = "Reports", controller = "Scroll" },
                constraints: new { action = @"^Reports$", reportYear = @"^\d{4}$", numberScroll = @"^\d{1,7}$", httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            ).DataTokens["UseNamespaceFallback"] = false;
            context.MapRoute(
                name: "DetailsRoutesWithParams",
                url: "{area}/{controller}/{action}/{numberScroll}/{reportYear}/{page}/{*filters}",
                defaults: new { area = "Nomenclature", action = "ScrollDetails", controller = "Scroll", page = 1, filters = UrlParameter.Optional },
                constraints: new { reportYear = @"^\d{4}$", page = @"^\d+$", numberScroll = @"^\d{1,7}$" },
                //constraints: page = new CompoundRouteConstraint(new IRouteConstraint[] {MinRouteConstraint(1),IntRouteConstraint(),
                //customConstraint = new UserAgentConstraint("Chrome")},
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "ReportRoutesGeneral",
                url: "{area}/{controller}/{action}/{reportName}",
                defaults: new { area = "Nomenclature", action = "Reports", controller = "Scroll" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "NomenclatureScroll_default",
                url: "{area}/{controller}/{page}",
                constraints: new { httpMethod = new HttpMethodConstraint("GET"), page = @"\d+" },
                defaults: new { area = "Nomenclature", action = "Index", controller = "Scroll", page = UrlParameter.Optional },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "General",
                url: "Nomenclature/{controller}/{action}",
                defaults: new { action = "Index", controller = "Scroll" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
        }
    }
}