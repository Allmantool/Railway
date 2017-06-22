using Ninject.Modules;
using NaftanRailway.BLL.Concrete.BussinesLogic;
using NaftanRailway.BLL.Concrete.AuthorizationLogic;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using System.Data.Entity;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.BLL.Services.IoC {
    public class NinjectRegistrations : NinjectModule {
        /// <summary>
        /// used for registering types into container
        /// </summary>
        public override void Load() {
            Bind<IBussinesEngage>().To<BussinesEngage>();
            Bind<IRailwayModule>().To<RailwayModule>();
            Bind<INomenclatureModule>().To<NomenclatureModule>();
            Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
            //log4Net
            //Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.DeclaringType));
            //_kernel.Bind<ISessionStorage>().To<SessionStorage>();

            Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
                new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });
        }
    }
}