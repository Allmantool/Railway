using System;
using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.BLL.DTO.Admin {
    public class MessageDTO {
        //[Key]
        //public int Id { get; set; }
        public DateTime SendTime { get; set; }
        public UserDTO User { get; set; }
        public string MsgText { get; set; }
    }
}