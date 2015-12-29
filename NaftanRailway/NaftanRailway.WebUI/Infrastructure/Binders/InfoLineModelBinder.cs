using System.Web.Mvc;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.Infrastructure.Binders {
    public class InfoLineModelBinder :IModelBinder {
        public object BindModel(ControllerContext controllerContext,ModelBindingContext bindingContext) {

            var request = controllerContext.HttpContext.Request;

            InfoLineViewModel model = (InfoLineViewModel)bindingContext.Model ?? new InfoLineViewModel();

            //model = GetValue(bindingContext, "");
            var form = request.Form;
            var sh = bindingContext.ValueProvider.GetValue("DocumentPackLine.Shipping");
            var wr = bindingContext.ValueProvider.GetValue("DocumentPackLine.Warehouse");
            var nw = bindingContext.ValueProvider.GetValue("DocumentPackLine.WagonsNumbers.[0]");
            var ls = bindingContext.ValueProvider.GetValue("DocumentPackLine.WagonsNumbers");
            
            InfoLineViewModel infoLine = new InfoLineViewModel() {
                DocumentPackLine = new ShippingInfoLine() {
                }
            };

            return infoLine;
        }

        private string GetValue(ModelBindingContext context,string name) {
            name = (context.ModelName == "" ? "" : context.ModelName + ".") + name;

            ValueProviderResult result = context.ValueProvider.GetValue(name);
            if (result == null || result.AttemptedValue == "") {
                return "<Not Specified>";
            } else {
                return (string)result.AttemptedValue;
            }
        }
    }
}

