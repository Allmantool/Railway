using System;
using System.Security.Principal;

namespace NaftanRailway.BLL.DTO.Admin {
    public class ADGroupDTO {
        public Guid Guid { get; set; }
        public SecurityIdentifier Sid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Sam { get; set; }
    }
}
