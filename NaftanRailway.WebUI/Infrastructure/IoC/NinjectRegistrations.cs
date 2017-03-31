using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Concrete.AuthorizationLogic;
using NaftanRailway.BLL.Concrete.BussinesLogic;
using Ninject.Modules;

namespace NaftanRailway.WebUI.Infrastructure.IoC {
    public class NinjectRegistrations : NinjectModule {
        /// <summary>
        /// used for registering types into container
        /// </summary>
        public override void Load() {
            //Bind<IBussinesEngage>().To<BussinesEngage>();
            //Bind<IRailwayModule>().To<RailwayModule>();
            //Bind<INomenclatureModule>().To<NomenclatureModule>();
            //Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
            //_kernel.Bind<ISessionStorage>().To<SessionStorage>();

            //Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
            //    new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });
        }
    }
}