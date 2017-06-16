using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    public class AuthorizeADAttribute : AuthorizeAttribute {
        public string Groups { get; set; }
        public string DenyUsers { get; set; }

        public AuthorizeADAttribute() {
            //avoid null reference exception
            Groups = string.Empty;
            DenyUsers = string.Empty;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (base.AuthorizeCore(httpContext)) {
                /* Return true immediately if the authorization is not locked down to any particular AD group */
                if (String.IsNullOrEmpty(Groups))
                    return true;

                // Get the AD groups
                var groups = Groups.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> users = DenyUsers.ToLower().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                string identity = httpContext.User.Identity.Name;

                //deny user
                if (users.Contains(identity.ToLower())) {
                    return false;
                }

                var contextType = httpContext.Request.IsLocal ? ContextType.Machine : ContextType.Domain;

                using (var ctx = new PrincipalContext(contextType)) {
                    var userPrinc = UserPrincipal.FindByIdentity(ctx, identity);

                    // Verify that the user is in the given AD group (if any)
                    try {
                        foreach (var group in groups)
                            if (userPrinc != null && userPrinc.IsMemberOf(ctx, IdentityType.Name, group))
                                return true;
                    } catch (Exception e) {
                        Console.WriteLine(e);
                        return false;
                    }

                    return false;
                }
            }

            return false;
        }
    }
}