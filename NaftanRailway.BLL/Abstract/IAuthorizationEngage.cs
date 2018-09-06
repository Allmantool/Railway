namespace NaftanRailway.BLL.Abstract {
    using System;
    using System.Collections.Generic;
    using NaftanRailway.BLL.DTO.Admin;

    public interface IAuthorizationEngage : IDisposable {
        ADUserDTO AdminPrincipal(string identity, bool isLocal = false);

        IEnumerable<ADUserDTO> GetMembers(string identity, int limit = 0, bool isLocal = false);
    }
}