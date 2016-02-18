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
        /*The MVC Framework will call the BindModel method when it wants an instance of the model type that the binder supports.
         * The parameters to the BindModel method are a ControllerContext object that you can use to get details of the 
            current request and a ModelBindingContext object, which provides details of the model object that is sought, as well 
            as access to the rest of the model binding facilities in the MVC application
         *  Model => Returns the model object passed to the UpdateModel method if binding has been invoked manually
            ModelName => Returns the name of the model that is being bound
            ModelType  => Returns the type of the model that is being created
            ValueProvider  => Returns an IValueProvider implementation that can be used to get data values from the request
         */
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            var form = controllerContext.HttpContext.Request.Form;
            //from form value provider
            if(form.Count > 0) {
                InputMenuViewModel menuForm = new InputMenuViewModel {
                    ReportPeriod = DateTime.Parse(form.Get("ReportPeriod")),
                    ShippingChoise = form.Get("ShippingChoise")
                };
                return menuForm;
            }

            var route = controllerContext.RouteData;

            //from route value provider
            if(route.Values.Keys.Contains("reportPeriod")) {
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