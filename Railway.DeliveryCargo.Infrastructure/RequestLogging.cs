using LibOwin;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Railway.DeliveryCargo.Infrastructure
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class RequestLogging
    {
        public static AppFunc Middleware(AppFunc next, ILogger log)
        {
            return async env =>
            {
                var owinContext = new OwinContext(env);

                log.LogInformation($"Incoming request: " +
                    $"@{owinContext.Request.Method} " +
                    $"@{owinContext.Request.Path} " +
                    $"@{owinContext.Request.Headers}");

                await next(env);

                log.LogInformation($"Incoming response: " +
                   $"@{owinContext.Response.StatusCode} " +
                   $"@{owinContext.Response.Headers}");
            };
        }
    }
}
