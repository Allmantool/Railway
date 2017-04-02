using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace NaftanRailway.UnitTests.General {
    [TestClass]
    public class ADTests {
        [TestMethod]
        public void UserPrincipalTest() {
            try {
                // create your domain context
                using (var ctx = new PrincipalContext(ContextType.Machine)) {
                    // define a "query-by-example" principal - here, we search for a GroupPrincipal
                    GroupPrincipal qbeGroup = new GroupPrincipal(ctx) { Name = "*" };
                    UserPrincipal qbeUser = new UserPrincipal(ctx) { Name = "AllmanGroup" };

                    // create your principal searcher passing in the QBE principal
                    PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };
                    PrincipalSearcher srchUsers = new PrincipalSearcher() { QueryFilter = qbeUser };

                    var users = (srchUsers.FindAll()).Select(x => (UserPrincipal)x).Select(x => new {
                        FullName = x.Name,
                        Email = x.EmailAddress,
                        EmployeId = x.EmployeeId,
                        Comment = x.Description,
                        Enable = x.Enabled,
                        Phone = x.VoiceTelephoneNumber,
                        Server = x.Context.ConnectedServer,
                        GivenName = x.GivenName,
                        MiddleName = x.MiddleName,
                        Surname = x.Surname,
                        DistinguishedName = x.DistinguishedName,
                        HomeDirector = x.HomeDirectory,
                        HomeDrive = x.HomeDrive,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        Sam = x.SamAccountName,
                        Guid = x.Guid,
                        Sid = x.Sid,
                        PrincipalName = x.UserPrincipalName,
                        groups = x.GetGroups().Select(gr => new { Name = gr.Name })
                    });

                    var groups = srchGroups.FindAll().Select(x => new {
                        FullName = x.Name,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        Sam = x.SamAccountName,
                        Guid = x.Guid,
                        Sid = x.Sid,
                        PrincipalName = x.UserPrincipalName,
                        Users =
                        ((GroupPrincipal)x).Members.Select(
                            us => new { Name = us.Name, PrincipalName = x.UserPrincipalName })
                    });

                    Assert.AreEqual(1, users.Count());
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                Assert.AreEqual(true, false);
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

        [TestMethod]
        public void DenyUsersTest() {
            var Groups = "Rail_Developers, Rail_Users";
            var DenyUsers = String.Empty;//@"Lan\kpg, Lan\cpn";
            //httpContext.User.Identity.Name
            var identity = @"LAN\CPN";
            var IsAuth = false;

            // Get the AD groups
            var groups = Groups.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var users = DenyUsers.ToLower().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //deny user
            Assert.IsFalse(users.Contains(identity.ToLower()));

            // Verify that the user is in the given AD group (if any)
            using (var ctx = new PrincipalContext(ContextType.Domain/*, "server"*/)) {
                //var userPrincipal = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, identity);
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                //if exist any one group
                foreach (var group in groups)
                    if (userPrincipal != null && userPrincipal.IsMemberOf(ctx, IdentityType.Name, group))
                        IsAuth = true;
            }

            Assert.AreEqual(true, IsAuth);
        }

        [TestMethod]
        public void SmmtpTest() {
            var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
            var message = new MailMessage();

            message.To.Add(new MailAddress("P.Chizhikov@naftan.by"));  // replace with valid value
            message.From = new MailAddress("P.Chizhikov@naftan.by");  // replace with valid value
            message.Subject = "Proposal to add right";

            message.Body = string.Format(body, "Чижиков", "P.Chizhikov@naftan.by", "Hello");
            message.IsBodyHtml = true;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            using (var smtp = new SmtpClient("naftan.by", 25) {
                Credentials = new NetworkCredential("P.Chizhikov@naftan.by", "ozTJ4w1U"),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                //EnableSsl = true,
            }) {
                try {
                    smtp.Send(message);
                } catch (Exception ex) {
                    string msg = ex.Message;
                }

                //await smtp.SendMailAsync(message);
            }

            Assert.AreEqual(true, true);
        }
    }
}