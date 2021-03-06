﻿using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.WebUI.ViewModels {
    public class LoginViewModel {
        [Required]
        [Display(Name = @"Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = @"Пароль")]
        public string Password { get; set; }
    }
}