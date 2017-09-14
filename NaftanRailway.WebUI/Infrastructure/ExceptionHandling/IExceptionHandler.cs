using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace NaftanRailway.WebUI.Infrastructure.ExceptionHandling {
    /// <summary>
    /// Exception handlers are the solution for customizing all possible responses to unhandled exceptions caught by Web API.
    /// </summary>
    public interface IExceptionHandler {
        Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken);
    }
}
