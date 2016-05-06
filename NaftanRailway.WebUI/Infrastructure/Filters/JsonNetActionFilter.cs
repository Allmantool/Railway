using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    public class JsonNetActionFilter : ActionFilterAttribute {
        /// <summary>
        /// After this is done we need to make sure our new version of JsonResult is being used. 
        /// The easiest way to do this is to create an action filter that executes after the action method is done. 
        /// In this action method we filter out all calls that have a result of type JsonResult and change that to a result of type JsonNetResult. 
        /// Since JsonNetResult extends JsonResult it works really simple.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            if(filterContext.Result.GetType()==typeof(JsonResult)) {
                //Get the standard result object with unserialized data
                JsonResult result = filterContext.Result as JsonResult;

                //Replace it with our new result object and transfer serttings
                if (result != null)
                    filterContext.Result = new JsonNetResult() {
                        ContentEncoding = result.ContentEncoding,
                        ContentType = result.ContentType,
                        Data = result.Data,
                        JsonRequestBehavior = result.JsonRequestBehavior
                    };

                /*Later on when ExecuteResult will be called it will be the function in JsonNetResult
                 instead of in JsonResult
                 */
                base.OnActionExecuted(filterContext);
            }
        }

    }
}