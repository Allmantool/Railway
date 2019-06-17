using LibOwin;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Railway.DeliveryCargo.Infrastructure
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class PerformanceLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log)
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
