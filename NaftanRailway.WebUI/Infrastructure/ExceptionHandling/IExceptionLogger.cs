using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    /// <summary>
    /// Exception loggers are the solution to seeing all unhandled exception caught by Web API.
    /// </summary>
    public interface IExceptionLogger {
        Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken);
    }
}
