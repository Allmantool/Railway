using System;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// View model. For passing information about paging between model and view throught action
    /// </summary>
    public class PagingInfo {
        public long TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages {
            get { return (int)Math.Ceiling((decimal)this.TotalItems / this.ItemsPerPage); }
        }
        public AjaxOptions AjaxOptions { get; set; }
        /// <summary>
        /// Can be use for client side js routing
        /// </summary>
        public RouteValueDictionary RoutingDictionary { get; set; }
    }
}