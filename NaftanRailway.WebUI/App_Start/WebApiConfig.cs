using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace NaftanRailway.WebUI {
    public static class WebApiConfig {
        public static void Register(HttpConfiguration config) {
            //IoC container
            //config.DependencyResolver = new NinjectDependencyResolver();

            //attribute routing
            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //The WebApiConfig.cs file is used to configure Web API rather than the Global.asax.cs file, and the statements
            //that Visual Studio adds by default configure the url routes that are used to process requests
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // configure json formatter
            JsonMediaTypeFormatter jsonFormatter = config.Formatters.JsonFormatter;

            var settings = new JsonSerializerSettings {
                //looping resolve
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                //enum json conversion
                Formatting = Formatting.Indented
            };

            jsonFormatter.SerializerSettings = settings;
            jsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }
    }
}