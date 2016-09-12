using System.Web.Mvc;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels.AuthorizationLogic;
using NaftanRailway.WebUI.ViewModels;


namespace NaftanRailway.WebUI.Controllers {
    public class AccountController : Controller {
        private readonly IAuthorizationEngage _engage;

        public AccountController(IAuthorizationEngage engage) {
            _engage = engage;
        }

        /// <summary>
        /// What is interesting is sign in and log in. Well, both mean same that you enter somewhere where you are already registered. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ViewResult Login() {
            //Functionality expansion
            if (Request.IsAuthenticated) {
                return View();
            }
            //log in
            return View();
        }

        /// <summary>
        /// Well, sign up simply means to register. It could be portal, newsletter or things the like. 
        /// So when you visit and access anything for the first time, you need to sign up.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public ViewResult SingUp(){
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl) {
            if (ModelState.IsValid && _engage.Login(model.UserName, model.Password)) {
                _engage.ChangeUserStatus(true, model.UserName);

                return Redirect(returnUrl ?? Url.Action("Index", "Ceh18"));
            }

            ModelState.AddModelError("", @"Указаны неверные учётные данные пользователя");

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ViewResult Register() {
            RegistrationViewModel model = new RegistrationViewModel {
                UsersList = _engage.GetInfoLines
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Register(RegistrationViewModel model) {
            model.UsersList = _engage.GetInfoLines;

            if (ModelState.IsValid) {
                if (_engage.Register(model)) {
                    model.UsersList = _engage.GetInfoLines;
                    TempData["message"] = string.Format("Пользователь {0} успешно зарегистрирован под ролью {1}", model.UserName, model.Role);
                    return View(model);
                } else {
                    ModelState.AddModelError("", @"Пользователь с таким профилем уже существует. Пожалуйста введите корректные данные");

                    return View(model);
                }
            }

            ModelState.AddModelError("", @"Указаны неверные учётные данные пользователя");

            return View(model);
        }

        public ActionResult Logout() {
            _engage.ChangeUserStatus(false, User.Identity.Name);

            _engage.Logout();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteUser(int userId) {
            if (_engage.DeleteUserById(userId))
                TempData["message"] = string.Format("Пользователь {0} успешно удален", userId);
            else
                TempData["message"] = string.Format("Ошибка при удаление пользователя {0}", userId);

            return RedirectToAction("Register", "Account");
        }
    }
}
