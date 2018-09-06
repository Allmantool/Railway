using Microsoft.AspNet.SignalR.Hubs;
using Ninject;

namespace NaftanRailway.WebUI.Hubs {
    public class HubActivator : IHubActivator {
        private readonly IKernel _container;

        public HubActivator(IKernel container) {
            this._container = container;
        }

        public IHub Create(HubDescriptor descriptor) {
            return (IHub)this._container.GetService(descriptor.HubType);
        }
    }
}