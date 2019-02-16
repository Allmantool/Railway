

namespace NaftanRailway.UnitTests.General {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using BLL.Abstract;
    using BLL.DTO.Admin;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Troubleshoot 1355 doesn't get access to domain.
    /// Can't visit doamins not trusting to you domain
    /// </summary>
    [TestClass]
    public class ADTests {
        [TestMethod]
        public void UserPrincipalTest() {
            try {
                var isLocal = false;
                var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
                var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";
                var searchName = isLocal ? "AllmanGroup" : "Чижиков*";
                const string identity = @"Lan\cpn";

                for (int i = 0; i < 15; i++) {
                    // create your domain context
                    PrincipalContext ctx = null;
                    using (ctx = new PrincipalContext(ctxType, hostDomain)) {
                        // define a "query-by-example" principal - here, we search for a GroupPrincipal
                        var qbeGroup = new GroupPrincipal(ctx) { Name = "*Sharepoint*" };
                        var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                        // create your principal searcher passing in the QBE principal
                        PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };

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
                    }
                }
            } catch (AppDomainUnloadedException appExc) {
                Console.WriteLine(appExc);
            } catch (Exception e) {
                Console.WriteLine(e);
                Assert.AreEqual(true, false);
            }

            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public void AdminPrincipal() {
            var isLocal = false;
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var container = isLocal ? null : "DC=lan,DC=naftan,DC=by";
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";

            ADUserDTO user = new ADUserDTO() { Name = "Anonymous", Description = "Not defy" };

            using (var ctx = new PrincipalContext(ctxType, hostDomain, container, ContextOptions.Negotiate)) {
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, @"lan/cpn");

                if (userPrincipal != null)
                {
                    user = new ADUserDTO {
                        FullName = userPrincipal.Name,
                        EmailAddress = userPrincipal.EmailAddress,
                        IdEmp = userPrincipal.EmployeeId != null ? 0 : Int32.Parse(userPrincipal.EmployeeId),
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
                            Guid = gr.Guid ?? new Guid(),
                            Users = this.GetMembers(gr.Name).ToList()
                        }).ToList()
                    };
                }
            }

            Assert.IsTrue(user != null);
        }

        [TestMethod]
        public IEnumerable<ADUserDTO> GetMembers(string identity, bool isLocal = false) {
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";
            var container = isLocal ? null : "DC=lan,DC=naftan,DC=by";

            using (var ctx = new PrincipalContext(ctxType, hostDomain, container, ContextOptions.Negotiate)) {
                // create your principal searcher passing in the QBE principal
                PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = new GroupPrincipal(ctx) { Name = identity } };

                var group = srchGroups.FindAll().OfType<GroupPrincipal>().Select(x => new ADGroupDTO {
                    Name = x.Name,
                    Description = x.Description,
                    Sam = x.SamAccountName,
                    Guid = x.Guid ?? new Guid(),
                    Sid = x.Sid,
                    Users = x.Members.OfType<UserPrincipal>().
                      Select(up => new ADUserDTO {
                          FullName = up.Name,
                          EmailAddress = up.EmailAddress,
                          //IdEmp = up.EmployeeId != null ? int.Parse(up.EmployeeId) : 0,
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
                          //Guid = up.Guid ?? new Guid(),
                          Sid = up.Sid,
                          PrincipalName = up.UserPrincipalName,
                          //Groups = up.GetGroups().Select(gr => new ADGroupDTO {
                          //    Name = gr.Name,
                          //    Description = gr.Description,
                          //    Sam = gr.SamAccountName,
                          //    Sid = gr.Sid,
                          //    Guid = gr.Guid ?? new Guid()
                          //}).ToList()
                      })
                }).ToList();

                return group.FirstOrDefault().Users.ToList();
            }
        }

        [TestMethod]
        public void WrkwithGrops() {
            var isLocal = false;
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";
            var container = isLocal ? null : "DC=lan,DC=naftan,DC=by";
            IList<ADGroupDTO> general = new List<ADGroupDTO>();
            IList<string> listOfGroups = new[] {
                //"*Sharepoint*"
                "Domain Users",
                "Сбыт_разработчики",
                "Internet_Power_Users",
                "Internet_Users",
                "Программисты ИВЦ",
                "REAL_Users",
                "Сбыт_операторы",
                "TS Clients Polymir",
                "naftania_users",
                "Internet_Stats_IVC",
                "экономист (Д 855)",
                "CZL_Client_Users",
                "ARME_Polz",
                "ORC_Admins",
                "ARM_EnergoRes_RWAccess",
                "Print_IVC-28-M712",
                "Directum_Users",
                "Rail_Developers",
                "OU_Pro-Department_Info_ROaccess"
            };

            using (var ctx = new PrincipalContext(ctxType, hostDomain, container, ContextOptions.Negotiate/**/)) {
                // create your principal searcher passing in the QBE principal
                foreach (var item in listOfGroups) {
                    PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = new GroupPrincipal(ctx) { Name = item } };

                    var group = srchGroups.FindAll().OfType<GroupPrincipal>().Select(gr => new ADGroupDTO {
                        Name = gr.Name,
                        Description = gr.Description,
                        Sam = gr.SamAccountName,
                        Guid = gr.Guid ?? new Guid(),
                        Sid = gr.Sid,
                        Users = gr.GetMembers(false).OfType<UserPrincipal>().
                        Where(us => us.UserPrincipalName == null ? false : us.UserPrincipalName.Contains("polymir") &&
                                    us.Context.Name == hostDomain && us.Context.ConnectedServer.Contains("naftan") &&
                                    us.DistinguishedName.Contains("DC=lan,DC=naftan,DC=by") &&
                                    us.Context.ConnectedServer.Contains(hostDomain)
                                    )
                        .Select(up => new ADUserDTO {
                            FullName = up.Name,
                            EmailAddress = up.EmailAddress,
                            //IdEmp = up.EmployeeId == null ? 0 : int.Parse(up.EmployeeId),
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
                            //Guid = up.Guid ?? new Guid(),
                            Sid = up.Sid,
                            PrincipalName = up.UserPrincipalName,
                            //Groups = up.GetGroups().Select(usGr => new ADGroupDTO {
                            //    Name = usGr.Name,
                            //    Description = usGr.Description,
                            //    Sam = usGr.SamAccountName,
                            //    Sid = usGr.Sid,
                            //    Guid = usGr.Guid ?? new Guid()
                            //})
                        })
                    });
                    var dto = group.FirstOrDefault();
                    //var filter = group.Where(x => x.Name.Contains("Readers"));
                    try {

                        Debug.WriteLine(
                            $"Group {dto.Name}, Total count user in current group: {(dto.Users == null ? 0 : dto.Users.Count())}");
                    } catch (Exception ex) {
                        Debug.WriteLine($"Group {dto.Name}, Exception: {ex.Message}");
                    }

                    general.Add(dto);
                }
            }
        }

        [TestMethod]
        public void UserInGroupTest() {
            bool result = false;
            // Get the AD groups
            var groups = string.Format("Rail_Developers, Domain Users, Internet_Users").Split(',').ToList();

            var identity = "Lan/cpn";

            using (var ctx = new PrincipalContext(ContextType.Domain)) {
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                foreach (var group in groups)
                {
                    if (userPrincipal != null && userPrincipal.IsMemberOf(ctx, IdentityType.Name, @group))
                    {
                        result = true;
                    }
                }
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

                // if exist any one group
                foreach (var group in groups)
                {
                    if (userPrincipal != null && userPrincipal.IsMemberOf(ctx, IdentityType.Name, group))
                    {
                        IsAuth = true;
                    }
                }
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

        [TestMethod]
        public void CheckValidationOnRemoteADDomain() {
            var isLocal = false;
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var container = isLocal ? null : "DC=lan,DC=naftan,DC=by";
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";

            ADUserDTO user = new ADUserDTO() { Name = "Anonymous", Description = "Not defy" };
            bool IsAuth = false;
            try {
                using (var ctx = new PrincipalContext(ContextType.Domain, hostDomain, null, ContextOptions.Negotiate)) {
                    IsAuth = ctx.ValidateCredentials(@"cpn", "1111");
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }

            Assert.AreEqual(true, IsAuth);
        }

        [TestMethod]
        public void MockAdminInfo() {
            var dtoColl = new[] {
                new ADUserDTO() {
                    Name = @"lan\cpn",
                    Guid = new Guid()
                },
                new ADUserDTO() {
                    Name = @"lan\cpn",
                     Guid = new Guid()
                },
            };

            // arrange
            var engMock = new Mock<IAuthorizationEngage>();
            engMock.Setup(m => m.AdminPrincipal(It.IsAny<string>(), false)).Returns<ADUserDTO>(dto => dtoColl.FirstOrDefault());

            // act

            // assert

        }
    }
}