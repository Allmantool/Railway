using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using NaftanRailway.BLL.DTO.Admin;

namespace NaftanRailway.Domain.BusinessModels.AuthorizationLogic {
    public class RegistrationViewModel {
        [Required]
        [Display(Name = @"Имя пользователя:")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = @"Пароль:")]
        public string Password { get; set; }

        [Required]
        [Display(Name = @"Email:")]
        [DataType(DataType.EmailAddress,ErrorMessage = @"Почтовый адрес неверен")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = @"Почтовый адрес неверен")]
        public string Email { get; set; }

        [Required]
        [Display(Name = @"Роль:")]
        public string Role { get; set; }

        [HiddenInput(DisplayValue = false)]
        public IEnumerable<SecurityInfoLineDTO> UsersList { get; set; }
    }
}