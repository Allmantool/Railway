using System;
using LibOwin;
using Railway.DeliveryCargo.Infrastructure.Core;
using Railway.DeliveryCargo.Infrastructure.Core.Consts;
using Serilog.Context;

namespace Railway.DeliveryCargo.Infrastructure
{
    public class CorrelationTokenMiddleware
    {
        public static AppFunc Invoke(AppFunc next)
        {
            return async env =>
            {
                Guid correlationToken;
                var owinContext = new OwinContext(env);

                if (!(owinContext.Request.Headers[HttpHeaders.CorrelationTokenKey] != null
                    && Guid.TryParse(owinContext.Request.Headers[HttpHeaders.CorrelationTokenKey], out correlationToken)))
                {
                    correlationToken = new Guid();
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
