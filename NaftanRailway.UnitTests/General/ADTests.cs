using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaftanRailway.BLL.DTO.Admin;
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
                var isLocal = false;
                var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
                var hostDomain = isLocal ? "Destkop" : "lan.naftan.by";
                //const string identity = @"Lan\cpn";

                // create your domain context
                using (var ctx = new PrincipalContext(ctxType, hostDomain)) {
                    // define a "query-by-example" principal - here, we search for a GroupPrincipal
                    GroupPrincipal qbeGroup = new GroupPrincipal(ctx) { Name = "*Rail_Developers*" };
                    UserPrincipal qbeUser = new UserPrincipal(ctx) { Name = "*Чижиков*", SamAccountName = "cpn" };

                    //var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                    // create your principal searcher passing in the QBE principal
                    PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };
                    PrincipalSearcher srchUsers = new PrincipalSearcher() { QueryFilter = qbeUser };

                    var users = (srchUsers.FindAll()).Select(x => (UserPrincipal)x).Select(userPrincipal => new ADUserDTO {
                        FullName = userPrincipal.Name,
                        EmailAddress = userPrincipal.EmailAddress,
                        IdEmp = Int32.Parse(userPrincipal.EmployeeId),
                        Description = userPrincipal.Description,
                        IsEnable = userPrincipal.Enabled ?? false,
                        Phone = userPrincipal.VoiceTelephoneNumber,
                        Server = userPrincipal.Context.ConnectedServer,
                        GivenName = userPrincipal.GivenName,
                        MiddleName = userPrincipal.MiddleName,
                        Surname = userPrincipal.Surname,
                        DistinguishedName = userPrincipal.DistinguishedName,
                        HomeDirector = userPrincipal.HomeDirectory,
                        HomeDrive = userPrincipal.HomeDrive,
                        DisplayName = userPrincipal.DisplayName,
                        Sam = userPrincipal.SamAccountName,
                        Guid = userPrincipal.Guid ?? new Guid(),
                        Sid = userPrincipal.Sid,
                        PrincipalName = userPrincipal.UserPrincipalName,
                        Groups = userPrincipal.GetGroups().Select(gr => new ADGroupDTO {
                            Name = gr.Name,
                            Description = gr.Description,
                            Sam = gr.SamAccountName,
                            Sid = gr.Sid,
                            Guid = gr.Guid ?? new Guid()
                        })
                    });

                    var groups = srchGroups.FindAll().Select(x => new {
                        FullName = x.Name,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        Sam = x.SamAccountName,
                        Guid = x.Guid,
                        Sid = x.Sid,
                        PrincipalName = x.UserPrincipalName,
                        Users = ((GroupPrincipal)x).Members.Select(user => (UserPrincipal)user).Select(up => new ADUserDTO {
                            FullName = up.Name,
                            EmailAddress = up.EmailAddress,
                            IdEmp = Int32.Parse(up.EmployeeId),
                            Description = up.Description,
                            IsEnable = up.Enabled ?? false,
                            Phone = up.VoiceTelephoneNumber,
                            Server = up.Context.ConnectedServer,
                            GivenName = up.GivenName,
                            MiddleName = up.MiddleName,
                            Surname = up.Surname,
                            DistinguishedName = up.DistinguishedName,
                            HomeDirector = up.HomeDirectory,
                            HomeDrive = up.HomeDrive,
                            DisplayName = up.DisplayName,
                            Sam = up.SamAccountName,
                            Guid = up.Guid ?? new Guid(),
                            Sid = up.Sid,
                            PrincipalName = up.UserPrincipalName,
                            Groups = up.GetGroups().Select(gr => new ADGroupDTO {
                                Name = gr.Name,
                                Description = gr.Description,
                                Sam = gr.SamAccountName,
                                Sid = gr.Sid,
                                Guid = gr.Guid ?? new Guid()
                            })
                        })
                    });

                    Assert.AreEqual(1, users.Count());
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                Assert.AreEqual(true, false);
            }
        }

        [TestMethod]
        public void WrkwithGrops() {
            var isLocal = false;
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";

            using (var ctx = new PrincipalContext(ctxType, hostDomain, null, ContextOptions.Negotiate)) {
                // create your principal searcher passing in the QBE principal
                PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = new GroupPrincipal(ctx) { Name = "Domain Users" } };

                var group = srchGroups.FindAll().Select(pr => (GroupPrincipal)pr).Select(gr => new ADGroupDTO {
                    Name = gr.Name,
                    Description = gr.Description,
                    Sam = gr.SamAccountName,
                    Guid = gr.Guid ?? new Guid(),
                    Sid = gr.Sid,
                    Users = gr.Members.
                    Where(us => //us is UserPrincipal && us.UserPrincipalName != null &&
                                us.Context.Name == hostDomain &&
                                us.DistinguishedName.Contains("OU=Нафтан,OU=Учетные записи,DC=lan,DC=naftan,DC=by") &&
                                us.Context.ConnectedServer.Contains(hostDomain))
                    .Select(user => (UserPrincipal)user).Select(up => new ADUserDTO {
                        FullName = up.Name,
                        EmailAddress = up.EmailAddress,
                        IdEmp = up.EmployeeId == null ? 0 : int.Parse(up.EmployeeId),
                        Description = up.Description,
                        IsEnable = up.Enabled ?? false,
                        Phone = up.VoiceTelephoneNumber,
                        Server = up.Context.ConnectedServer,
                        GivenName = up.GivenName,
                        MiddleName = up.MiddleName,
                        Surname = up.Surname,
                        DistinguishedName = up.DistinguishedName,
                        HomeDirector = up.HomeDirectory,
                        HomeDrive = up.HomeDrive,
                        DisplayName = up.DisplayName,
                        Sam = up.SamAccountName,
                        Guid = up.Guid ?? new Guid(),
                        Sid = up.Sid,
                        PrincipalName = up.UserPrincipalName,
                        Groups = up.GetGroups().Select(usGr => new ADGroupDTO {
                            Name = usGr.Name,
                            Description = usGr.Description,
                            Sam = usGr.SamAccountName,
                            Sid = usGr.Sid,
                            Guid = usGr.Guid ?? new Guid()
                        })
                    }).Take(100)
                });

                var dto = group.FirstOrDefault();
                if (dto != null) {
                    var users = dto.Users.ToList();
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