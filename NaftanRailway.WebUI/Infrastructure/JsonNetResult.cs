using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace NaftanRailway.WebUI.Infrastructure {
    /// <summary>
    /// Implementing JSon.net
    /// What we basically need to do is to extend the JsonResult class and create new functionality for the ExecuteResult method. 
    /// We then get the opportunity to select ourselves how we want to serialize the Json data and can thus use the JsonSerializer from Json.NET instead.
    /// </summary>
    public class JsonNetResult:JsonResult {
        public JsonSerializerSettings Settings { get;private set; }

        public JsonNetResult() {
            Settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //DateFormatString = "dd.MM.yyyy"
            };
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null)
                throw new ArgumentException("context");

            HttpResponseBase response = context.HttpContext.Response;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data == null)
                return;

            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            var scriptSerializer = JsonSerializer.Create(Settings);

            //Serialize the data to the Output stream of the response
            scriptSerializer.Serialize(response.Output,Data);
        }
    }
}