using System.Web.Http.ExceptionHandling;
using log4net;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    /// <summary>
    /// logs any exception in the application
    /// </summary>
    public class UnhandledExceptionLogger : ExceptionLogger {
        public static ILog Logger { get; private set; }

        public UnhandledExceptionLogger(ILog logger = null) : base() {
            Logger = logger;
        }

        public override void Log(ExceptionLoggerContext context) {
            var logMsg = context.Exception.ToString();

            if (Logger != null) {
                Logger.Debug(logMsg);
            }

            //Do whatever logging you need to do here.
        }
    }
}