using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.ValueProvider {
   public class CustomValueProviderFactory : ValueProviderFactory {
       /*The GetValueProvider method is called when the model binder wants to obtain values for the binding process.
        This implementation simply creates and returns an instance of the CountryValueProvider class, but you can use 
            the data provided through the ControllerContext parameter to respond to different kinds of requests by creating 
            different value providers.
        */
        public override IValueProvider GetValueProvider(ControllerContext
                controllerContext) {
            return new CountryValueProvider();
        }
    }
}