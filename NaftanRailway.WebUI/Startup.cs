using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NaftanRailway.WebUI.Startup))]
namespace NaftanRailway.WebUI {
    public class Startup {
        public void Configuration(IAppBuilder app) {
            //var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            //var monitor = new PresenceMonitor(heartBeat);

            //monitor.StartMonitoring();

            GlobalHost.Configuration.DefaultMessageBufferSize = 100;
            app.MapSignalR();
            //GlobalHost.HubPipeline.RequireAuthentication();
        }
    }
}