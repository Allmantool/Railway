﻿using Ninject;
using System.Web.Http.Dependencies;
using System;
using System.Collections.Generic;

namespace NaftanRailway.WebUI.Infrastructure.DI {
    public class NinjectDependencyResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel) {
            _kernel = kernel;
        }

        public IDependencyScope BeginScope() {
            return this;
        }

        public void Dispose() {
        }

        public object GetService(Type serviceType) {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return _kernel.GetAll(serviceType);
        }
    }
}
