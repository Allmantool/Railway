using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Admin;
using System.Collections.Generic;

namespace NaftanRailway.BLL.Concrete.AuthorizationLogic {
    public class AuthorizationEngage : IAuthorizationEngage {
        public ADUserDTO AdminPrincipal(string identity, bool isLocal = false) {
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;

            using (var ctx = new PrincipalContext(ctxType)) {
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                if (userPrincipal != null)
                    return new ADUserDTO {
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
                        }).ToList()
                    };
            }

            return new ADUserDTO() { Name = "Anonymous", Description = "Not defy" };
        }

        public IEnumerable<ADUserDTO> GetMembers(string identity, bool isLocal = false) {
            var ctxType = isLocal ? ContextType.Machine : ContextType.Domain;

            using (var ctx = new PrincipalContext(ctxType)) {
                GroupPrincipal qbeGroup = new GroupPrincipal(ctx) { Name = identity };

                // create your principal searcher passing in the QBE principal
                PrincipalSearcher srchGroups = new PrincipalSearcher() { QueryFilter = qbeGroup };

                var group = srchGroups.FindAll().Select(x => new ADGroupDTO {
                    Name = x.Name,
                    Description = x.Description,
                    Sam = x.SamAccountName,
                    Guid = x.Guid ?? new Guid(),
                    Sid = x.Sid,
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
                        }).ToList()
                    })
                }).ToList();

                return group.FirstOrDefault().Users.ToList();
            }
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}