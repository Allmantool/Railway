using System;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
    //[AllowAnonymous]
    public abstract class BaseController : Controller {
        /// <summary>
        /// Data transfer object for AD user principal
        /// </summary>
        public class ADUserDTO {
            public string Name { get; set; }
            public string DomainName { get; set; }
            public string EmailAddress { get; set; }
        }

        /// <summary>
        /// current AD user
        /// </summary>
        public ADUserDTO CurrentADUser {
            get {
                if (Request.IsLocal) return new ADUserDTO { Name = "Local work (Admin;)", DomainName = @"LAN\CPN", EmailAddress = "@mail" };

                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                string identity = User.Identity.Name;

                using (ctx) {
                    var user = UserPrincipal.FindByIdentity(ctx, identity);

                    if (user != null)
                        return new ADUserDTO { Name = user.DisplayName, DomainName = user.Name, EmailAddress = user.EmailAddress };
                }
                return null;
            }
        }

        public string GetGroupsName { get { return GetADInfo(); } }

        public string BrowserInfo {
            get { return GetBrowserInfo(); }
        }

        public BaseController() { }

        //Summarize information about user and environment
        private string GetBrowserInfo() {
            var browser = Request.Browser;
            var userName = User.Identity.Name;
            var totalOnlineUsers = (int)@HttpContext.Application["TotalOnlineUsers"];

            var result = String.Format(
                 "Browser: {0} {1},<br />EcmaScript: {2},<br />JavaScript: {3},<br />Platform: {4}," +
                 "<br />Cookies: {5},<br />ActiveXControls: {6},<br />JavaApplets {7},<br />Frames: {8}," +
                 "<br />User Name: {9}{10},<br />Online: {11}",
             browser.Browser,
             browser.Version,
             browser.EcmaScriptVersion,
             browser["JavaScriptVersion"],
             browser.Platform,
             browser.Cookies,
             browser.ActiveXControls,
             browser.JavaApplets,
             browser.Frames,
             string.Format("{0} ({1})", CurrentADUser.Name, CurrentADUser.EmailAddress),
             userName.Length == 0 ? "" : string.Format("({0})", userName.Replace(@"\", "&#92;")),
             totalOnlineUsers
            );

            return result;
        }

        private string GetADInfo() {
            //SAM(англ.Security Account Manager) Диспетчер учётных записей безопасности — RPC - сервер Windows, оперирующий базой данных учетных записей.

            var result = "";
            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            // define a "query-by-example" principal - here, we search for a GroupPrincipal
            GroupPrincipal qbeGroup = new GroupPrincipal(ctx) { Name = "*" };
            UserPrincipal qbeUser = new UserPrincipal(ctx) { Name = "*Чижиков П.Н*" };

            // create your principal searcher passing in the QBE principal
            PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };
            PrincipalSearcher srchUsers = new PrincipalSearcher(qbeUser);

            // find all matches
            foreach (var found in srchUsers.FindAll()) {
                // do whatever here - "found" is of type "Principal" - it could be user, group, computer.....
                foreach (var item in found.GetGroups()) {
                    result = result + ", " + ((GroupPrincipal)item).DisplayName;
                }
                //result = result + ", " + found.;
            }

            return result;
        }
    }
}