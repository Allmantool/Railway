using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.Concrete.AuthorizationLogic;
using NaftanRailway.BLL.Concrete.BussinesLogic;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;
using Ninject.Modules;
using System.Data.Entity;

namespace NaftanRailway.WebUI.Infrastructure.DI {
    public class NinjectRegistrations : NinjectModule {
        /// <summary>
        /// used for registering types into container
        /// </summary>
        public override void Load() {
            Bind<IBussinesEngage>().To<BussinesEngage>();
            Bind<IRailwayModule>().To<RailwayModule>();
            Bind<INomenclatureModule>().To<NomenclatureModule>();
            Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
            //_kernel.Bind<ISessionStorage>().To<SessionStorage>();

            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
                new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });
        }
    }
}
