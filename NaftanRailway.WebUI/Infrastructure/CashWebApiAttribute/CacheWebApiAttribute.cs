using System;
using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace NaftanRailway.WebUI.Infrastructure.CashWebApiAttribute {
    /// <summary>
    /// It's custom cashing for web api ([CacheWebApi(Duration = 3600)])
    /// </summary>
    public class CacheWebApiAttribute : ActionFilterAttribute {
        public int Duration { get; set; }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext) {
            filterContext.Response.Headers.CacheControl = new CacheControlHeaderValue() {
                MaxAge = TimeSpan.FromMinutes(Duration),
                MustRevalidate = true,
                Private = true
            };
        }
    }
}