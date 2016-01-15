using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Infrastructure.Binders {
    /// <summary>
    /// Return InputMenuViewModel from form and routing value providers
    /// </summary>
    public class InputMenuModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            var form = controllerContext.HttpContext.Request.Form;

            if (form.Count > 0) {
                InputMenuViewModel menuForm = new InputMenuViewModel {
                    ReportPeriod = DateTime.Parse(form.Get("ReportPeriod")),
                    ShippingChoise = form.Get("ShippingChoise") 
                };
                return menuForm;
            }

            var route = controllerContext.RouteData;

            if (route.Values.Keys.Contains("reportPeriod")) {
                InputMenuViewModel menuRoute = new InputMenuViewModel {
                    ReportPeriod = DateTime.ParseExact((string)route.Values["reportPeriod"], "MMyyyy", CultureInfo.InvariantCulture),
                    ShippingChoise = (string)route.Values["templateNumber"] 
                };
                return menuRoute;
            }

            return new InputMenuViewModel();
        }
    }
}