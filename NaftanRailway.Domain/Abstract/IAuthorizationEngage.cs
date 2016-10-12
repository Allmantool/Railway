using System.Collections.Generic;
using NaftanRailway.Domain.BusinessModels.AuthorizationLogic;
using System;

namespace NaftanRailway.Domain.Abstract {
    public interface IAuthorizationEngage : IDisposable {
        /// <summary>
        /// Retrive information about all current authenticating users
        /// </summary>
        IEnumerable<SecurityInfoLine> GetInfoLines { get; }
        bool ChangeUserStatus(bool isActive, string userName);
        /// <summary>
        /// Delete user by id (Cascading deleting) + delete support information
        /// </summary>
        /// <param name="userId"></param>
        bool DeleteUserById(int userId);
        bool Login(string username, string password);
        bool Logout();
        bool Register(RegistrationViewModel model);
    }
}