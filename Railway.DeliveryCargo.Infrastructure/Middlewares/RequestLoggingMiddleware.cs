namespace Railway.DeliveryCargo.Infrastructure
{
    using Microsoft.Extensions.Logging;
    using Railway.DeliveryCargo.Infrastructure.Core;

    public class RequestLoggingMiddleware
    {
        public static AppFunc Invoke(AppFunc next, ILogger log)
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
