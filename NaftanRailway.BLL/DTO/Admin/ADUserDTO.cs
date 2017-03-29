using System.Collections.Generic;

namespace NaftanRailway.BLL.DTO.Admin {
    /// <summary>
    /// Data transfer object for AD user principal
    /// </summary>
    public class ADUserDTO {
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string DomainName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Sam { get; set; }
        public string PrincipalName { get; set; }
        public IEnumerable<string> Groups { get; set; }

        public ADUserDTO() {
            FullName = "Full Name";
            Name = "Local work (Admin;)";
            DomainName = @"LAN\CPN";
            EmailAddress = "@mail";
        }
    }
}