using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using log4net;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Admin;

namespace NaftanRailway.BLL.Concrete.AdminLogic {
    public class AuthorizationEngage : IAuthorizationEngage {
        public ILog Log { get; private set; }

        public AuthorizationEngage(ILog logger) {
            this.Log = logger;
        }

        public ADUserDTO AdminPrincipal(string identity, bool isLocal = false) {
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";
            var container = isLocal ? null : "DC=lan,DC=naftan,DC=by";

            ADUserDTO user = new ADUserDTO() { Name = "Anonymous", Description = "Not defy" };

            try {
                using (var ctx = new PrincipalContext(ctxType, hostDomain, container, ContextOptions.Negotiate)) {
                    var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                    if (userPrincipal != null)
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
                                //Users = GetMembers(gr.Name, 50).ToList()
                            }).ToList()
                        };
                }
            } catch (Exception ex) {
                this.Log.DebugFormat(@"Исключение при попытке работы с AD: {0}", ex.Message);
            }
            return user;
        }

        public IEnumerable<ADUserDTO> GetMembers(string identity, int limit = 0, bool isLocal = false) {
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;
            var hostDomain = isLocal ? "DESKTOP-LHO63TH" : "lan.naftan.by";
            var container = isLocal ? null : "DC=lan,DC=naftan,DC=by";

            try {
                using (var ctx = new PrincipalContext(ctxType/*, hostDomain, container, ContextOptions.Negotiate*/)) {
                    // create your principal searcher passing in the QBE principal
                    PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = new GroupPrincipal(ctx) { Name = identity } };

                    var group = srchGroups.FindAll().Select(x => new ADGroupDTO {
                        Name = x.Name,
                        Description = x.Description,
                        Sam = x.SamAccountName,
                        Guid = x.Guid ?? new Guid(),
                        Sid = x.Sid,
                        Users = ((GroupPrincipal)x).Members.OfType<UserPrincipal>().
                        //Where(us => //us is UserPrincipal && us.UserPrincipalName != null &&
                        //us.Context.Name == hostDomain &&
                        //us.DistinguishedName.Contains("OU=Нафтан,OU=Учетные записи,DC=lan,DC=naftan,DC=by") &&
                        //us.Context.ConnectedServer.Contains(hostDomain)).
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
                    });

                    return limit == 0 ? group.FirstOrDefault().Users.ToList() : group.FirstOrDefault().Users.ToList().Take(limit);
                }
            } catch (Exception ex) {
                this.Log.DebugFormat(@"Исключение при попытке работы с AD: {0}", ex.Message);
                return new[] { new ADUserDTO() { Name = "Unknown" } };
            }
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}