﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Web.Http;

namespace NaftanRailway.WebUI.Hubs {
    public abstract class SignalRBase<THub> : ApiController where THub : IHub {
        private readonly Lazy<IHubContext> _hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        );
        protected IHubContext Hub {
            get { return this._hub.Value; }
        }
    }
}