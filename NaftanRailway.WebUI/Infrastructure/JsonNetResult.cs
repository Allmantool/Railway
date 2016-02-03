using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace NaftanRailway.WebUI.Infrastructure {
    public class JsonNetResult:JsonResult {
        public JsonSerializerSettings Settings { get; private set; }

        public JsonNetResult() {
            Settings = new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
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

            var scriptSerializer = JsonSerializer.Create(this.Settings);

            //Serialize the data to the Output stream of the response
            scriptSerializer.Serialize(response.Output,this.Data);
        }
    }
}