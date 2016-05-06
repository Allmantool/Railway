﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels.AuthorizationLogic;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContext;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using Ninject;

namespace NaftanRailway.WebUI.Infrastructure {
    public class NinjectDependencyResolver : IDependencyResolver {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel) {
            _kernel = kernel;
            AddBindings();
        }

        /// <summary>
        /// Put bindings here
        /// </summary>
        private void AddBindings() {
            _kernel.Bind<IBussinesEngage>().To<BussinesEngage>();

            _kernel.Bind<IUnitOfWork>().To<UnitOfWork>()
                .WithConstructorArgument("contexts", new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });

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