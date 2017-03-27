using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.Filters {
    public class AuthorizeADAttribute : AuthorizeAttribute {
        public string Groups { get; set; }
        public string DenyUsers { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (base.AuthorizeCore(httpContext)) {
                /* Return true immediately if the authorization is not locked down to any particular AD group */
                if (String.IsNullOrEmpty(Groups))
                    return true;

                // Get the AD groups
                var groups = Groups.Split(',').ToList();

                var identity = httpContext.User.Identity.Name;

                //deny user
                if (DenyUsers.ToLower().Split(',').Contains(identity.ToLower())) return false;

                // Verify that the user is in the given AD group (if any)
                var ctx = new PrincipalContext(ContextType.Domain/*, "server"*/);

                using (ctx) {
                    //var userPrincipal = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, identity);
                    var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                    foreach (var group in groups)
                        if (userPrincipal != null && userPrincipal.IsMemberOf(ctx, IdentityType.Name, @group))
                            return true;
                }
            }

            return false;
        }
    }
}