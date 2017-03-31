using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Http.Dependencies;

namespace NaftanRailway.WebUI.Infrastructure.DI {
    public class NinjectDependencyScope : IDependencyScope {
        private IResolutionRoot resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver) {
            Contract.Assert(resolver != null);

            this.resolver = resolver;
        }

        public void Dispose() {
            var disposable = this.resolver as IDisposable;
            if (disposable != null) {
                disposable.Dispose();
            }

            this.resolver = null;
        }

        public object GetService(Type serviceType) {
            if (this.resolver == null) {
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            }

            return this.resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            if (this.resolver == null) {
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            }

            return this.resolver.GetAll(serviceType);
        }
    }
}
