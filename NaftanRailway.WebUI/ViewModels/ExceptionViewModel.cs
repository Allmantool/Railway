using System;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.ViewModels {
    public class ExceptionViewModel {
            public ExceptionContext Model { get; set; }
            public Tuple<string, string>[] Modules { get; set; }
    }
}