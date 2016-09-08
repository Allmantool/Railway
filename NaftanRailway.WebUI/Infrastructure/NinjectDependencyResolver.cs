using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;
using Ninject;

namespace NaftanRailway.WebUI.Infrastructure {
    public class NinjectDependencyResolver : IDependencyResolver {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel) {
            _kernel = kernel;
            AddBindings();
        }

        /// <summary>
        /// Put bindings of dependecy injection here
        /// </summary>
        private void AddBindings() {
            _kernel.Bind<IBussinesEngage>().To<BussinesEngage>();
            _kernel.Bind<IRailwayModule>().To<RailwayModule>();
            _kernel.Bind<INomenclatureModule>().To<NomenclatureModule>();

            _kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
                new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });

            //_kernel.Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
        }

        public object GetService(Type serviceType) {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return _kernel.GetAll(serviceType);
        }
    }
}