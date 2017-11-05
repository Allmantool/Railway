using Ninject.Web.Common.WebHost;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NaftanRailway.WebUI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NaftanRailway.WebUI.App_Start.NinjectWebCommon), "Stop")]

namespace NaftanRailway.WebUI.App_Start {
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System.Web.Mvc;
    using Ninject.Modules;
    using Infrastructure.DI;
    using BLL.Services.IoC;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using Hubs;
    using log4net;

    public static class NinjectWebCommon {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop() {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel() {
            //var kernel = new StandardKernel();
            NinjectModule registrations = new NinjectRegistrations();
            var kernel = new StandardKernel(registrations);

            try {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                var ninjectResolver = new NinjectDependencyResolver(kernel);

                //In some strange reasons i didn't need set resolver for web api, in other case i got circle reference. Maybe this is because i install some web api nuget package
                DependencyResolver.SetResolver(ninjectResolver); // MVC
                GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new HubActivator(kernel)); //SignalR
                //GlobalConfiguration.Configuration.DependencyResolver = ninjectResolver; //Web api
                return kernel;
            } catch {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) {
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new HubActivator(kernel));

            //log4Net
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.DeclaringType));
        }
    }
}