using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace NaftanRailway.BLL.DTO.Admin {
    /// <summary>
    /// Data transfer object for AD user principal
    /// </summary>
    public class ADUserDTO {
        [Key]
        public int IdEmp { get; set; }
        public Guid Guid { get; set; }
        public SecurityIdentifier Sid { get; set; }
        public string Description { get; set; }
        public bool IsEnable { get; set; }
        public string Server { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
        public string DistinguishedName { get; set; }
        public string HomeDirector { get; set; }
        public string HomeDrive { get; set; }
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string DomainName { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string Sam { get; set; }
        public string PrincipalName { get; set; }
        public IEnumerable<ADGroupDTO> Groups { get; set; }
    }
}