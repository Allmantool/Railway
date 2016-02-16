﻿using System;

namespace NaftanRailway.WebUI.ViewModels {
    /// <summary>
    /// View model. For passing information about paging between model and view throught action
    /// </summary>
    public class PagingInfo {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}