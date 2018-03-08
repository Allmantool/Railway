using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace NaftanRailway.WebUI.Infrastructure {
    /// <summary>
    /// Implementing JSon.net
    /// What we basically need to do is to extend the JsonResult class and create new functionality for the ExecuteResult method. 
    /// We then get the opportunity to select ourselves how we want to serialize the Json data and can thus use the JsonSerializer from Json.NET instead.
    /// </summary>
    public class JsonNetResult : JsonResult {
        public JsonSerializerSettings Settings { get; private set; }

        public JsonNetResult() {
            this.Settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                //DateFormatString = "dd.MM.yyyy"
            };
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null)
                throw new ArgumentException("context");

            HttpResponseBase response = context.HttpContext.Response;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;

            if (this.Data == null)
                return;

            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;
            response.ContentEncoding = Encoding.UTF8;

            var scriptSerializer = JsonSerializer.Create(this.Settings);

            //Serialize the data to the Output stream of the response
            scriptSerializer.Serialize(response.Output, this.Data);
        }
    }
}