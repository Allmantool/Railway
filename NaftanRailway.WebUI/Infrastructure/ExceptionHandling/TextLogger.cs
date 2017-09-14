using System.Diagnostics;
using System.Web.Http.ExceptionHandling;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    public class TextLogger : ExceptionLogger {
        //grab message from exception
        //public override void Log(ExceptionLoggerContext context) {
        //    Debug.WriteLine(string.Format("From the global logger: {0}", context.ExceptionContext.Exception.Message));

        //    base.Log(context);
        //}
    }
}