using System;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
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

        //Work with LDAP/ AD
        private string GetUserDisplayName() {
            if (HttpContext.Request.IsLocal) {
                return "Local work (Admin;)";
            }
            string identity = HttpContext.User.Identity.Name;

            string displayName = String.Empty;
            PrincipalContext context;

            //DS (1 of 5 AD service) = Domen services (user and recourses management (servers, net app, client)
            if (identity.Substring(0, 7).ToLower() == "polymir")
                context = new PrincipalContext(ContextType.Domain, "POLYMIR.NET");
            else
                context = new PrincipalContext(ContextType.Domain, "lan.naftan.by");

            using (context) {
                //Инкапсулирует участников, которые являются учетными записями пользователей.
                var principal = UserPrincipal.FindByIdentity(context, identity);

                displayName = principal.DisplayName;
            }

            return displayName;
        }

        //Summarize information about user and environment
        private string GetBrowserInfo() {
            var browser = Request.Browser;
            var userName = @User.Identity.Name;
            var totalOnlineUsers = (int)@HttpContext.Application["TotalOnlineUsers"];

            var result = String.Format(
                 "Browser: {0} {1},<br />EcmaScript: {2},<br />JavaScript: {3},<br />Platform: {4}," +
                 "<br />Cookies: {5},<br />ActiveXControls: {6},<br />JavaApplets {7},<br />Frames: {8}," +
                 "<br />User Name: {9}{10},<br />Online: {11},",
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
             userName.Length == 0 ? "" : String.Format("({0})", userName.Replace(@"\", "&#92;")),
             totalOnlineUsers
            );


            return result;
        }

        private string GetADInfo() {
            var result ="";
            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            // define a "query-by-example" principal - here, we search for a GroupPrincipal 
            GroupPrincipal qbeGroup = new GroupPrincipal(ctx);

            // create your principal searcher passing in the QBE principal    
            PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);

            // find all matches
            foreach (var found in srch.FindAll()) {
                // do whatever here - "found" is of type "Principal" - it could be user, group, computer.....   
                result = result + ", " + found.SamAccountName;
            }

            PrincipalContext ctx1 = new PrincipalContext(ContextType.Domain);
            // get the AD Group you are wanting to Query
            //GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx,"cn=Финансы_Программисты");
            //foreach (Principal p in group.Members) {
            //    result = result + ", " + p.Name;
            //}

            return result;
        }
    }
}