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

            if (identity.Substring(0, 7).ToLower() == "polymir")
                context = new PrincipalContext(ContextType.Domain, "POLYMIR.NET");
            else
                context = new PrincipalContext(ContextType.Domain, "lan.naftan.by");

            using (context) {
                var principal = UserPrincipal.FindByIdentity(context, identity);
                displayName = principal.DisplayName;
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
             userName.Length == 0 ? "" : String.Format("({0})", userName),
             totalOnlineUsers);

            return result;
        }

        /// <summary>
        /// Is current user has permissions to approve transport requests
        /// </summary>
        /// <param name="identity"></param>
        /// <returns>Tuple where Item1 - EmloyeeId, Item2 - is this user approver </returns>
        //public static Tuple<int, bool> IsApprover(string identity) {
        //    transportEntities _db = new transportEntities();

        //    //bool result = false;

        //    int employeeId = 0;
        //    PrincipalContext context = new PrincipalContext(ContextType.Domain, "lan.naftan.by");

        //    if (identity.Substring(0, 7).ToLower() == "polymir") //If user from Polymir than MAGIC
        //    {
        //        //парсить кусок логина на предмет табельного номера
        //        string tabN = identity.Substring(9, identity.Length - 9);
        //        int n = 0;
        //        for (int i = 0; i < tabN.Length; i++) {
        //            if (tabN.Substring(i, 1) != "0") {
        //                n = i;
        //                break;
        //            }
        //        }
        //        tabN = tabN.Substring(n, tabN.Length - n); //предполагаемый табельный номер

        //        var empl = _db.Employees.Where(e => e.EmployeeNumber == tabN).SingleOrDefault();
        //        if (empl != null)
        //            employeeId = empl.EmployeeId;
        //    } else //User from Naftan, do search in AD
        //      {
        //        string sIdMan = String.Empty;
        //        int idMan = 0;
        //        using (context) {
        //            var principal = UserPrincipal.FindByIdentity(context, identity);
        //            sIdMan = principal.EmployeeId;
        //        }
        //        Int32.TryParse(sIdMan, out idMan);
        //        var empl = _db.Employees.Where(e => e.id_men == idMan).SingleOrDefault();
        //        if (empl != null)
        //            employeeId = empl.EmployeeId;
        //        else //Если полимировец с логином из домена LAN
        //        {
        //            //парсить кусок логина на предмет табельного номера
        //            string tabN = identity.Substring(5, identity.Length - 5);
        //            int n = 0;
        //            for (int i = 0; i < tabN.Length; i++) {
        //                if (tabN.Substring(i, 1) != "0") {
        //                    n = i;
        //                    break;
        //                }
        //            }
        //            tabN = tabN.Substring(n, tabN.Length - n); //предполагаемый табельный номер
        //            var empl1 = _db.Employees.Where(e => e.EmployeeNumber == tabN).SingleOrDefault();
        //            if (empl1 != null)
        //                employeeId = empl1.EmployeeId;
        //        }
        //    }

        //    if (employeeId == 0)
        //        return Tuple.Create(employeeId, false);

        //    bool isApprover = _db.RequestApprovers.Any(ap => ap.EmployeeId == employeeId);

        //    return Tuple.Create(employeeId, isApprover);
        //}
    }
}