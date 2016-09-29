using System.Collections.Generic;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    public class QuickDialogMenuModel {
        public string Title { get; set; }
        public string Location { get; set; }
        public EnumMenuOperation TypeOperation { get; set; }
        public IDictionary<string,object> HtmlAttrs { get; set; }
        public AjaxOptions OptionsAjax { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}