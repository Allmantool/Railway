using System;
<<<<<<< HEAD
using System.Web.Mvc.Ajax;
=======
>>>>>>> 5a6dfbf73a7198e3ac3178486d5927a4a853fb17

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// View model. For passing information about paging between model and view throught action
    /// </summary>
    public class PagingInfo {
        public long TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
<<<<<<< HEAD
        public int TotalPages {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
        public AjaxOptions AjaxOptions { get; set; }
=======

        public int TotalPages {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
>>>>>>> 5a6dfbf73a7198e3ac3178486d5927a4a853fb17
    }
}