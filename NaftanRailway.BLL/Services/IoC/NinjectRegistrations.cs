namespace NaftanRailway.BLL.Services.IoC {
    using System.Data.Entity;
    using Abstract;
    using Concrete.AuthorizationLogic;
    using Concrete.BussinesLogic;
    using Domain.Abstract;
    using Domain.Concrete;
    using Domain.Concrete.DbContexts.Mesplan;
    using Domain.Concrete.DbContexts.OBD;
    using Domain.Concrete.DbContexts.ORC;
    using Ninject.Modules;

    public class NinjectRegistrations : NinjectModule {
        /// <summary>
        /// used for registering types into container
        /// </summary>
        public override void Load() {
            this.Bind<IBussinesProvider>().To<BussinesProvider>();
            this.Bind<IRailwayModule>().To<RailwayModule>();
            this.Bind<INomenclatureModule>().To<NomenclatureModule>();
            this.Bind<IAuthorizationEngage>().To<AuthorizationEngage>();
            //log4Net
            //Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.DeclaringType));
            //_kernel.Bind<ISessionStorage>().To<SessionStorage>();

            this.Bind<IUnitOfWork>().To<UnitOfWork>().WithConstructorArgument("contexts",
                new DbContext[] { new OBDEntities(), new MesplanEntities(), new ORCEntities() });
        }
    }
}