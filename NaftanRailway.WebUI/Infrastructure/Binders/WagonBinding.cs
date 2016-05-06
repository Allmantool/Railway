using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Binders {
    public class WagonBinding : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var q = controllerContext.HttpContext.Request.Form;

            return new object();
        }
    }
}