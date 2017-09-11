using System.Web.Mvc;

namespace NaftanRailway.WebUI.Areas.Admin {
    public class AdminAreaRegistration : AreaRegistration {
        public override string AreaName {
            get {
                //GlobalConfiguration.Configuration.MapHttpAttributeRoutes();
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { Controller = "Managment", action = "ADStructure", id = UrlParameter.Optional }
            );
        }
    }
}
