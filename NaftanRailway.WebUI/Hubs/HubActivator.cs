using Microsoft.AspNet.SignalR.Hubs;
using Ninject;

namespace NaftanRailway.WebUI.Hubs {
    public class HubActivator : IHubActivator {
        private readonly IKernel container;

        public HubActivator(IKernel container) {
            this.container = container;
        }

        public IHub Create(HubDescriptor descriptor) {
            //var service = (IHub)DependencyResolver.Current.GetService(descriptor.HubType); 
            return (IHub)container.GetService(descriptor.HubType);
        }
    }
}