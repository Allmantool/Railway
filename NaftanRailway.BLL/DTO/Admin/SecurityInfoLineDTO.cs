namespace NaftanRailway.BLL.DTO.Security {
    /// <summary>
    /// One info row(line)
    /// </summary>
    public class SecurityInfoLineDTO {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string RoleName { get; set; }

        public string Email { get; set; }

        public bool IsConfirmed { get; set; }
    }
}