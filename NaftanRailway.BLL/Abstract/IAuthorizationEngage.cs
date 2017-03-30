using NaftanRailway.BLL.DTO.Admin;
using System;

namespace NaftanRailway.BLL.Abstract {
    public interface IAuthorizationEngage : IDisposable {
        ADUserDTO AdminPrincipal(string identity);
    }
}