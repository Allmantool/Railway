using System;
using System.Collections.Generic;
using Ninject;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Concrete.BussinesLogic;
using NaftanRailway.BLL.Concrete.AuthorizationLogic;

namespace NaftanRailway.WebUI.Infrastructure.DI {
    //public class CustomNinjectDependencyResolver : IDependencyResolver {
    //    private readonly IKernel _kernel;

    //    public CustomNinjectDependencyResolver() : this(new StandardKernel()) {

    //    }

    //    public CustomNinjectDependencyResolver(IKernel kernel) {
    //        _kernel = kernel;
    //        AddBindings();
    //    }

    //    /// <summary>
    //    /// Put bindings of dependecy injection here
    //    /// </summary>
    //    private void AddBindings() {
    //        _kernel.Bind<IBussinesEngage>().To<BussinesEngage>();
    //        _kernel.Bind<IRailwayModule>().To<RailwayModule>();
    //        _kernel.Bind<INomenclatureModule>().To<NomenclatureModule>();
    //        _kernel.Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
    //        //_kernel.Bind<ISessionStorage>().To<SessionStorage>();

    //        //_kernel.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
    //        //    new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });
    //    }

    //    public object GetService(Type serviceType) {
    //        return _kernel.TryGet(serviceType);
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType) {
    //        return _kernel.GetAll(serviceType);
    //    }
    //}
}