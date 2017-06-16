using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.BLL.DTO.Admin {
    public class UserDTO {
        [Key]
        public string ConnectionId { get; set; }
        public string Name { get; set; }
    }
}
