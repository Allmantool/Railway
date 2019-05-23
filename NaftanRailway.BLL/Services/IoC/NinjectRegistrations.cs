using NaftanRailway.BLL.Concrete.AdminLogic;
using NaftanRailway.BLL.Concrete.BusinessLogic;
using Railway.Core.Data.EF;
using Railway.Core.Data.Interfaces;

namespace NaftanRailway.BLL.Services.IoC {
    using System.Data.Entity;
    using Abstract;
    using Ninject.Modules;

    public class NinjectRegistrations : NinjectModule {
        /// <summary>
        /// used for registering types into container
        /// </summary>
        public override void Load() {
            this.Bind<IBusinessProvider>().To<BusinessProvider>();

            this.Bind<IRailwayModule>().To<RailwayModule>();
            this.Bind<INomenclatureModule>().To<NomenclatureModule>();
            this.Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
            //log4Net
            //Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.DeclaringType));
            //_kernel.Bind<ISessionStorage>().To<SessionStorage>();


            // this.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
            //    new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });
        }
    }
}