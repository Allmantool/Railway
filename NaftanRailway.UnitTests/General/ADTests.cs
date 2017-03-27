using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace NaftanRailway.UnitTests.General {
    [TestClass]
    public class ADTests {
        [TestMethod]
        public void UserPrincipalTest() {
            // create your domain context
            using (var ctx = new PrincipalContext(ContextType.Domain)) {
                // define a "query-by-example" principal - here, we search for a GroupPrincipal
                GroupPrincipal qbeGroup = new GroupPrincipal(ctx) { Name = "*" };
                UserPrincipal qbeUser = new UserPrincipal(ctx) { Name = "*" };

                // create your principal searcher passing in the QBE principal
                PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };
                PrincipalSearcher srchUsers = new PrincipalSearcher(qbeUser);

                var users = srchUsers.FindAll().Select(x => new {
                    Name = x.Name,
                    Sam = x.SamAccountName,
                    Guid = x.Guid,
                    Sid = x.Sid,
                    PrincipalName = x.UserPrincipalName,
                    groups = x.GetGroups().Select(gr => new { Name = gr.Name })
                });

                var groups = srchUsers.FindAll().SelectMany(x => x.GetGroups().Select(gr => new {
                    Name = gr.Name,
                    Sam = gr.SamAccountName,
                    Guid = gr.Guid,
                    Sid = gr.Sid,
                    PrincipalName = gr.UserPrincipalName
                }));
            }
        }

        [TestMethod]
        public void UserInGroupTest() {
            bool result = false;
            // Get the AD groups
            var groups = string.Format("Rail_Developers, Domain Users, Internet_Users").Split(',').ToList();

            var identity = "Lan/cpn";//httpContext.User.Identity.Name;
            // Verify that the user is in the given AD group (if any)

            //var userPrincipal = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, identity);
            using (var ctx = new PrincipalContext(ContextType.Domain)) {
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                foreach (var group in groups)
                    if (userPrincipal != null && userPrincipal.IsMemberOf(ctx, IdentityType.Name, @group))
                        result = true;
            }

            Assert.AreEqual(true, result);
        }
    }
}