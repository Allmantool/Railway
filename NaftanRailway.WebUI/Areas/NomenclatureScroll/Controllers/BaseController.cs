using System;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Controllers {
    /// <summary>
    /// Use BaseController
    //  It is recommended to use Base Controller with our Controller and this Base Controller will inherit Controller class directly.
    /// It provides isolation space between our Controller[InterviewController] and Controller.
    /// Using Base Controller, we can write common logic which could be shared by all Controllers.
    /// (Auth, exception logic)
    /// Although in basic controller we have properties for access to httpContextObjec
    /// </summary>
    public abstract class BaseController : Controller {
        /// <summary>
        /// Get AD user name
        /// </summary>
        public string ADUserName {
            get { return GetUserDisplayName(); }
        }

        public string BrowserInfo {
            get { return GetBrowserInfo(); }
        }

        private string GetUserDisplayName() {
            if (HttpContext.Request.IsLocal) {
                return "Local work (Admin;)";
            }
            string identity = HttpContext.User.Identity.Name;

            string displayName = String.Empty;
            PrincipalContext context;

            switch (identity.Substring(0, 7).ToLower()) {
                case "polymir":
                    context = new PrincipalContext(ContextType.Domain, "POLYMIR.NET");
                    break;
                default:
                    context = new PrincipalContext(ContextType.Domain, "lan.naftan.by");
                    break;
            }

            using (context) {
                var principal = UserPrincipal.FindByIdentity(context, identity);
                if (principal != null) displayName = principal.DisplayName;
            }

            return displayName;
        }

        private string GetBrowserInfo() {
            var browser = Request.Browser;
            var userName = @User.Identity.Name;
            var totalOnlineUsers = (int)@HttpContext.Application["TotalOnlineUsers"];

            var result = String.Format(
                 "Browser: {0} {1},<br />EcmaScript: {2},<br />JavaScript: {3},<br />Platform: {4}," +
                 "<br />Cookies: {5},<br />ActiveXControls: {6},<br />JavaApplets {7},<br />Frames: {8}," +
                 "<br />User Name: {9}{10},<br />Online: {11}.",
             browser.Browser,
             browser.Version,
             browser.EcmaScriptVersion.ToString(),
             browser["JavaScriptVersion"],
             browser.Platform,
             browser.Cookies,
             browser.ActiveXControls,
             browser.JavaApplets,
             browser.Frames,
             ADUserName,
             //&#92; => html5 escape character '/'
             userName.Length == 0 ? "" : string.Format("({0})", userName.Replace(@"\", "&#92;")),
             totalOnlineUsers);

            return result;
        }
    }
}