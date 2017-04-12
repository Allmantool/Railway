using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NaftanRailway.WebUI.Startup))]
namespace NaftanRailway.WebUI {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            //GlobalHost.DependencyResolver.Register(typeof(AdminHub), () => new AdminHub(new AuthorizationEngage()));
            app.MapSignalR();
        }
    }
}