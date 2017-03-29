using NaftanRailway.BLL.DTO.Admin;
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Controllers {
    /// <summary>
    /// Use BaseController
    //  It is recommended to use Base Controller with our Controller and this Base Controller will inherit Controller class directly.
    /// It provides isolation space between our Controller[InterviewController] and Controller.
    /// Using Base Controller, we can write common logic which could be shared by all Controllers.
    /// (Auth, exception logic)
    /// Although in basic controller we have properties for access to httpContextObjec
    /// </summary>
    //[AllowAnonymous]
    public abstract class BaseController : Controller {
        /// <summary>
        /// current AD user
        /// </summary>
        public ADUserDTO CurrentADUser {
            get {
                if (Request.IsLocal) return new ADUserDTO();

                string identity = User.Identity.Name;

                var domainHost = identity.Substring(0, 7).ToLower() == "polymir" ? "POLYMIR.NET" : "lan.naftan.by";

                using (var ctx = new PrincipalContext(ContextType.Domain, domainHost)) {
                    var user = UserPrincipal.FindByIdentity(ctx, identity);

                    if (user != null)
                        return new ADUserDTO {
                            FullName = user.Name,
                            Domain = ctx.Name,
                            Name = user.DisplayName,
                            EmailAddress = user.EmailAddress,
                            Phone = user.VoiceTelephoneNumber,
                            Sam = user.SamAccountName,
                            PrincipalName = user.UserPrincipalName,
                            Groups = user.GetGroups().Select(gr => gr.Name).ToList()
                        };
                }
                return null;
            }
        }

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
    }
}