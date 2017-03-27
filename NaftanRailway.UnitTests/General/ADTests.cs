using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace NaftanRailway.UnitTests.General {
    [TestClass]
    public class ADTests {
        [TestMethod]
        public void UserPrincipalTest() {
            var result = "";
            // create your domain context
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            using (ctx) {
                // define a "query-by-example" principal - here, we search for a GroupPrincipal
                GroupPrincipal qbeGroup = new GroupPrincipal(ctx) { Name = "*" };
                UserPrincipal qbeUser = new UserPrincipal(ctx) { Name = "*Чижиков П.Н*" };

                // create your principal searcher passing in the QBE principal
                PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };
                PrincipalSearcher srchUsers = new PrincipalSearcher(qbeUser);

                // find all matches
                foreach (var found in srchUsers.FindAll()) {
                    // do whatever here - "found" is of type "Principal" - it could be user, group, computer.....
                    var info = (UserPrincipal)found;
                    var groups = found.GetGroups();
                    foreach (var item in found.GetGroups()) {
                        result = result + ", " + ((GroupPrincipal)item).DisplayName;
                    }
                    //result = result + ", " + found.;
                }
            }
        }

        [TestMethod]
        public void UserInGroupTest() {
            bool result = false;
            // Get the AD groups
            var groups = string.Format("Rail_Developers, Domain Users, Internet_Users").Split(',').ToList();

            var identity = "Lan/cpn";//httpContext.User.Identity.Name;
            // Verify that the user is in the given AD group (if any)
            var ctx = new PrincipalContext(ContextType.Domain/*, "server"*/);

            //var userPrincipal = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, identity);
            using (ctx) {
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                foreach (var group in groups)
                    if (userPrincipal != null && userPrincipal.IsMemberOf(ctx, IdentityType.Name, @group))
                        result = true;
            }

            Assert.AreEqual(true, result);

        }
    }
}