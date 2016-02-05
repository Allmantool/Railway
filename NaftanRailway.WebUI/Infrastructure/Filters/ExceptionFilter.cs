using System;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    public class ExceptionFilter : FilterAttribute, IExceptionFilter {
        public void OnException(ExceptionContext filterContext) {
            if(!filterContext.ExceptionHandled && filterContext.Exception is ArgumentOutOfRangeException) {
                filterContext.Result = new RedirectResult("~/Views/Shared/Error.cshtml");
            }
        }
    }
}