using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels.AuthorizationLogic;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContext;
using Ninject;

namespace NaftanRailway.WebUI.Infrastructure {
    public class NinjectDependencyResolver :IDependencyResolver {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel) {
            _kernel = kernel;
            AddBindings();
        }

        /// <summary>
        /// Put bindings here
        /// </summary>
        private void AddBindings() {
            _kernel.Bind<IDocumentsRepository>().To<EFDocumentsRepository>();
            _kernel.Bind<ISessionDbRepository>().To<EFSessioinDbRepository>();
            _kernel.Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
        }

        public object GetService(Type serviceType) {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return _kernel.GetAll(serviceType);
        }
    }
}