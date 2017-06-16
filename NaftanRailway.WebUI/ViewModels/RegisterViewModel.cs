using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NaftanRailway.WebUI.ViewModels {
    public class RegisterViewModel {
        [Required]
        [Display(Name = @"Имя пользователя:")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = @"Пароль:")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        [Display(Name = @"Почтовый адрес:")]
        public string Email { get; set; }
    }
}