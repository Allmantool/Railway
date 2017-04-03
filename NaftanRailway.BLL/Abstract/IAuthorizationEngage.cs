using NaftanRailway.BLL.DTO.Admin;
using System;
using System.Collections.Generic;

namespace NaftanRailway.BLL.Abstract {
    public interface IAuthorizationEngage : IDisposable {
        ADUserDTO AdminPrincipal(string identity, bool isLocal = false);

        IEnumerable<ADUserDTO> GetMembers(string identity, bool isLocal = false);
    }
}