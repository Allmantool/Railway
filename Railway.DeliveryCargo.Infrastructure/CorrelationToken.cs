using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;
using Railway.DeliveryCargo.Infrastructure.Core.Consts;
using Serilog.Context;

namespace Railway.DeliveryCargo.Infrastructure
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class CorrelationToken
    {
        public static AppFunc Middleware(AppFunc next)
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
