using System.Web.Mvc;
using NaftanRailway.WebUI.Infrastructure.Filters;

// ReSharper disable once CheckNamespace
namespace Filters {
    public static class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
            /*The final thing we need to do is to register our new ActionFilter in App_Start/FilterConfig.cs 
             * so it’ll be activated for all action methods automatically.
             Now, everytime you execute an action method according to the first initial example above, 
             * Json.NET serializer will be used instead of the standard JavaScriptSerializer.*/
            filters.Add(new JsonNetActionFilter());
        }
    }
}