using System.Web.Mvc;
using NaftanRailway.BLL.Services;

namespace NaftanRailway.WebUI.Infrastructure.Binders {
    public class StorageTableModelBinder : IModelBinder {
        private const string SessionKey = "SessionStorage";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

            SessionStorage storage = null;

            //get Storage from session
            if (controllerContext.HttpContext.Session != null) {
                storage = (SessionStorage)controllerContext.HttpContext.Session[SessionKey];
            }

            //create the Storage if there wasn't one in the session data
            if (storage == null) {
                storage = new SessionStorage();

                //save in session
                if (controllerContext.HttpContext.Session != null) {
                    controllerContext.HttpContext.Session[SessionKey] = storage;
                }
            }

            return storage;
        }
    }
}