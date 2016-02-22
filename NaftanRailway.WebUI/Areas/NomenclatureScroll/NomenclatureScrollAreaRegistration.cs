using System.Web.Mvc;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll {
    public class NomenclatureScrollAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                return "NomenclatureScroll";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                name: "ReportRoutesWithParams",
                url: "Nomenclature/{controller}/{action}/{reportName}/{numberScroll}/{reportYear}",
                defaults: new { action = "Reports", controller ="Scroll" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "DetailsRoutesWithParams",
                url: "Nomenclature/{controller}/{action}/{numberScroll}/{reportYear}",
                defaults: new { action = "ScrollDetails", controller ="Scroll" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "ReportRoutesGeneral",
                url: "Nomenclature/{controller}/{action}/{reportName}",
                defaults: new { action = "Reports", controller ="Scroll" },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
            context.MapRoute(
                name: "NomenclatureScroll_default",
                url: "Nomenclature/{controller}/{page}",
                defaults: new { action = "Index", controller ="Scroll", page = UrlParameter.Optional },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
        }
    }
}