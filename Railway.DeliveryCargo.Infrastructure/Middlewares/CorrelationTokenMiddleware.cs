namespace Railway.DeliveryCargo.Infrastructure
{
    using System;
    using Railway.DeliveryCargo.Infrastructure.Core;
    using Railway.DeliveryCargo.Infrastructure.Core.Consts;
    using Serilog.Context;

    public class CorrelationTokenMiddleware
    {
        public static AppFunc Invoke(AppFunc next)
        {
            return async env =>
            {
                var owinContext = new OwinContext(env);

                if (!(owinContext.Request.Headers[HttpHeaders.CorrelationTokenKey] != null
                    && Guid.TryParse(owinContext.Request.Headers[HttpHeaders.CorrelationTokenKey], out var correlationToken)))
                {
                    correlationToken = default(Guid);
                }

                owinContext.Set(HttpHeaders.CorrelationTokenKey, correlationToken.ToString());

                using (LogContext.PushProperty(HttpHeaders.CorrelationTokenKey, correlationToken))
                {
                    await next(env);
                }
            };
        }
    }
}
