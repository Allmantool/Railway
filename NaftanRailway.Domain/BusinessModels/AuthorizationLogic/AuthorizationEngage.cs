using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.Domain.Concrete.DbContext.Security;
using WebMatrix.WebData;

namespace NaftanRailway.Domain.BusinessModels.AuthorizationLogic {
    public class AuthorizationEngage : IAuthorizationEngage {
        private readonly UnitOfWork _unitOfWork;

        public AuthorizationEngage() {
            _unitOfWork = new UnitOfWork(new SimpleMemberShipDbEntities());
        }

        public IEnumerable<SecurityInfoLine> GetInfoLines {
            get {
                return from up in _unitOfWork.Repository<UserProfile>().Get_all()
                       join wMs in _unitOfWork.Repository<webpages_Membership>().Get_all() on up.UserId equals wMs.UserId
                       select new SecurityInfoLine {
                           UserId = up.UserId,
                           Email = up.Email,
                           IsConfirmed = wMs.IsConfirmed.Value,
                           RoleName = up.webpages_Roles.FirstOrDefault().RoleName,
                           UserName = up.UserName,
                       };
            }
        }
        public bool ChangeUserStatus(bool isActive, string userName) {
            var firstOrDefault = _unitOfWork.Repository<UserProfile>().Get_all().FirstOrDefault(us => us.UserName == userName);

            if(firstOrDefault != null)
                firstOrDefault.IsActive = isActive;

            _unitOfWork.Save();
            return true;
        }
        public bool DeleteUserById(int userId) {

            var delUser = _unitOfWork.Repository<UserProfile>().Get(x => x.UserId == userId);
            var delRoles =_unitOfWork.Repository<webpages_Roles>().Get(x => x.UserProfiles.Select(y => y.UserId).Contains(userId));

            if(delUser!=null && !delUser.IsActive) {
                //At Start delete relantship (pk_key) (many to many link table)
                delUser.webpages_Roles.Remove(delRoles);
                delRoles.UserProfiles.Remove(delUser);

                //Delete user
                _unitOfWork.Repository<UserProfile>().Delete(delUser);

                //Delete row from webpages_OAuthMembership
                _unitOfWork.Repository<webpages_OAuthMembership>().Delete(x => x.UserId == userId);

                //Delete row from webpages_Membership
                _unitOfWork.Repository<webpages_Membership>().Delete(x => x.UserId == userId);

                _unitOfWork.Save();

                return true;
            }

            return false;
        }
        public bool Login(string username, string password) {
            bool result = WebSecurity.Login(username, password);

            return result;
        }
        public bool Logout() {
            bool result = true;

            try { WebSecurity.Logout(); }
            catch(Exception) {
                result = false;
            }

            return result;
        }
        public bool Register(RegistrationViewModel model) {
            bool result = true;

            try {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password, propertyValues: new { model.Email });
                Roles.AddUserToRole(model.UserName, model.Role);
            }
            catch(MembershipCreateUserException) {
                result = false;
            }
            return result;
        }
    }
}
