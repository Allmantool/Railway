using System.Web.Mvc;
using System.Web.Routing;
//using System.Web.Mvc.Routing.Constraints;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll {
    public class NomenclatureScrollAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "NomenclatureScroll";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            //Attribute Routing in MVC5
            //routes.MapMvcAttributeRoutes();
            context.MapRoute(
                name: "ReportRoutesWithParams",
                url: "{area}/{controller}/{action}/{reportName}/{numberScroll}/{reportYear}/{*catchall}",
                defaults: new { area ="Nomenclature", action = "Reports", controller ="Scroll" },
                constraints: new { action = "^Reports$" , httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            ).DataTokens["UseNamespaceFallback"] = false;
            context.MapRoute(
                name: "DetailsRoutesWithParams",
                url: "{area}/{controller}/{action}/{numberScroll}/{reportYear}/{page}/{filters}",
                defaults: new { area = "Nomenclature", action = "ScrollDetails", controller = "Scroll", page = 1, filters = UrlParameter.Optional},
                constraints: new {page = @"\d+"},
                //constraints: page = new CompoundRouteConstraint(new IRouteConstraint[] {MinRouteConstraint(1),IntRouteConstraint(),
                //customConstraint = new UserAgentConstraint("Chrome")},
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name:"",
                url: "{area}/{controller}/{action}/{numberScroll}/{reportYear}/{page}",
                defaults: new { area = "Nomenclature", action = "ScrollDetails", controller = "Scroll", page = UrlParameter.Optional },
                constraints: new { page = @"\d+" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "ReportRoutesGeneral",    
                url: "{area}/{controller}/{action}/{reportName}",
                defaults: new { area = "Nomenclature", action = "Reports", controller ="Scroll" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "NomenclatureScroll_default",
                url: "{area}/{controller}/{page}",
                constraints: new {httpMethod = new HttpMethodConstraint("GET"), page = @"\d+" },
                defaults: new { area = "Nomenclature", action = "Index", controller ="Scroll", page = UrlParameter.Optional },
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