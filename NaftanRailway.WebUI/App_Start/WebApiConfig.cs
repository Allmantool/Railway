using System.Web.Http;

namespace NaftanRailway.WebUI {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {

            //config.MapHttpAttributeRoutes();

            //config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //The WebApiConfig.cs file is used to configure Web API rather than the Global.asax.cs file, and the statements
            //that Visual Studio adds by default configure the url routes that are used to process requests
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}