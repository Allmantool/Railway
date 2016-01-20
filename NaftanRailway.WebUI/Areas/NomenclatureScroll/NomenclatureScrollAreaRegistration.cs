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
                name: "NomenclatureScroll_default",
                url: "Nomenclature/{controller}/{action}/{page}",
                defaults: new { action = "Index",controller ="Scroll", page = UrlParameter.Optional },
                namespaces: new[] { "NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers" }
            );
        }
    }
}