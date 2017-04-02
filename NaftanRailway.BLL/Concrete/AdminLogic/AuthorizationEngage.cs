using System;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using NaftanRailway.BLL.Abstract;
using NaftanRailway.BLL.DTO.Admin;


namespace NaftanRailway.BLL.Concrete.AuthorizationLogic {
    public class AuthorizationEngage : IAuthorizationEngage {
        public ADUserDTO AdminPrincipal(string identity) {
            using (var ctx = new PrincipalContext(ContextType.Machine)) {
                var userPrincipal = UserPrincipal.FindByIdentity(ctx, identity);

                //if (userPrincipal != null)
                //    return new ADUserDTO {
                //        FullName = userPrincipal.Name,
                //        EmailAddress = userPrincipal.EmailAddress,
                //        IdEmp = Int32.Parse(userPrincipal.EmployeeId),
                //        Description = userPrincipal.Description,
                //        IsEnable = userPrincipal.Enabled ?? false,
                //        Phone = userPrincipal.VoiceTelephoneNumber,
                //        Server = userPrincipal.Context.ConnectedServer,
                //        GivenName = userPrincipal.GivenName,
                //        MiddleName = userPrincipal.MiddleName,
                //        Surname = userPrincipal.Surname,
                //        DistinguishedName = userPrincipal.DistinguishedName,
                //        HomeDirector = userPrincipal.HomeDirectory,
                //        HomeDrive = userPrincipal.HomeDrive,
                //        DisplayName = userPrincipal.DisplayName,
                //        Sam = userPrincipal.SamAccountName,
                //        Guid = userPrincipal.Guid ?? new Guid(),
                //        Sid = userPrincipal.Sid,
                //        PrincipalName = userPrincipal.UserPrincipalName,
                //        Groups = userPrincipal.GetGroups().Select(gr => new ADGroupDTO {
                //            Name = gr.Name,
                //            Description = gr.Description,
                //            Sam = gr.SamAccountName,
                //            Sid = gr.Sid,
                //            Guid = gr.Guid ?? new Guid()
                //        })
                //    };
            }

            return new ADUserDTO() { Name = "Anonymous", Description = "Not defy" };
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}