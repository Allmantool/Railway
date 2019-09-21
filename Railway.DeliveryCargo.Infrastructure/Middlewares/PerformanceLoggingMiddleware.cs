namespace Railway.DeliveryCargo.Infrastructure
{
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;
    using Railway.DeliveryCargo.Infrastructure.Core;

    public class PerformanceLoggingMiddleware
    {
        public static AppFunc Invoke(AppFunc next, ILogger log)
        {
            return async env =>
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                await next(env);

                stopWatch.Stop();

                var owinContext = new OwinContext(env);

                log.LogInformation($"Request: " +
                    $"@{owinContext.Request.Method} " +
                    $"@{owinContext.Request.Path} " +
                    $"executed in @{stopWatch.ElapsedMilliseconds:000} ms");
            };
        }
    }
}
