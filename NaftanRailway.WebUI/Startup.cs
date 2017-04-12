using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NaftanRailway.WebUI.Startup))]
namespace NaftanRailway.WebUI {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            app.MapSignalR();
        }
    }
}