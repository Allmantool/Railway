using System.Collections.Generic;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using NaftanRailway.BLL.Services;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.ViewsModels {
    public class QuickDialogMenuModel {
        public string Title { get; set; }
        public string Location { get; set; }
        public EnumMenuOperation TypeOperation { get; set; }
        public IDictionary<string,object> HtmlAttrs { get; set; }
        public AjaxOptions OptionsAjax { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}