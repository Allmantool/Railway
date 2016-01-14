using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.BusinessModels.AuthorizationLogic;
using NaftanRailway.WebUI.Controllers;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.UnitTest.Tests {
    [TestClass]
    public class AdminSecurityTests {
        //[TestMethod]
        //public void Can_Login_With_Valid_Credentials() {
        //    // Arrange - create a mock authentication provider
        //    Mock<IAuthorizationEngage> mock = new Mock<IAuthorizationEngage>();

        //    mock.Setup(m => m.Login("admin","1111")).Returns(true);

        //    // Arrange - create the view model
        //    LoginViewModel model = new LoginViewModel {
        //        UserName = "admin",
        //        Password = "1111"
        //    };

        //    // Arrange - create the controller
        //    AccountController target = new AccountController(null);

        //    //Act - authenticate using valid credentials
        //    //ActionResult result = target.Login(model,"/MyURL");

        //    // Assert
        //    Assert.IsInstanceOfType(result,typeof(RedirectResult));
        //    Assert.AreEqual("/MyURL",((RedirectResult)result).Url);
        //}

        //[TestMethod]
        //public void Cannot_Login_With_Invalid_Credentials() {
        //    // Arrange - create a mock authentication provider
        //     Mock<IAuthorizationEngage> mock = new Mock<IAuthorizationEngage>();
        //    mock.Setup(m => m.Login("badUser","badPass")).Returns(false);

        //    // Arrange - create the view model
        //    LoginViewModel model = new LoginViewModel {
        //        UserName = "badUser",
        //        Password = "badPass"
        //    };

        //    //Arrange - create the controller
        //    AccountController target = new AccountController(null);

        //    //Act - authenticate using valid credentials
        //    ActionResult result = target.Login(model,"/MyURL");

        //    //Assert
        //    Assert.IsInstanceOfType(result,typeof(ViewResult));
        //    Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        //}

        [TestMethod]
        public void Logout() {
            // Arrange - create a mock authentication provider
           Mock<IAuthorizationEngage> mock = new Mock<IAuthorizationEngage>();

            // Arrange - create the controller
            AccountController target = new AccountController(null);

            mock.Setup(m => m.Logout()).Returns(true);

            //Act - authenticate using valid credentials
            ActionResult result = target.Logout();

            // Assert
            Assert.IsInstanceOfType(result,typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Register() {
            //Arrange (create template security db and check hes properties
            Mock<IAuthorizationEngage> mock = new Mock<IAuthorizationEngage>();

            mock.Setup(sm => sm.GetInfoLines).Returns(new[] {
                new SecurityInfoLine() {
                    UserId = 1,UserName = "Pavel",Email = "One@tut.by",IsConfirmed = true,RoleName = "Admin"
                },
                new SecurityInfoLine() {
                    UserId = 2,UserName = "Natali",Email = "Two@tut.by",IsConfirmed = false,RoleName = "Registar"
                },
                new SecurityInfoLine() {
                    UserId = 3,UserName = "Roman",Email = "Three@tut.by",IsConfirmed = true,RoleName = "Accountant"
                }
            });

            RegistrationViewModel newUser = new RegistrationViewModel() {
                UserName = "New", Email = "New@mail.by", Password = "1111", Role = "Admin",UsersList = mock.Object.GetInfoLines
            };

            AccountController target = new AccountController(null);

            //Act
            RegistrationViewModel result = (RegistrationViewModel)target.Register().ViewData.Model;

            //Assert (Check data recordser in mock db_all user)
            Assert.AreEqual(result.UsersList,mock.Object.GetInfoLines);

            //Act (check model state RegistrationViewModel model)
            target.Register(null);

            //Assert (Check count recordser in mock db)
            Assert.AreEqual(target.ViewData.ModelState.IsValid,false);
            Assert.IsTrue(target.ViewData.ModelState.Count == 1,@"Указаны неверные учётные данные пользователя");

            //Act (check register method authProvider.Register(model))
            mock.Setup(m => m.Register(result)).Returns(false);
            target.Register(result);

            //Assert (Check count recordser in mock db)
            Assert.AreEqual(target.ViewData.ModelState.IsValid,false);
            Assert.IsTrue(target.ViewData.ModelState.Count == 1,@"Пользователь с таким профилем уже существует. Пожалуйста введите корректные данные");

            //Act (check send tempData)
            target.ViewData.ModelState.Clear();
            mock.Setup(m => m.Register(newUser)).Returns(true);

            target.Register(newUser);
            
            string msg = target.TempData.Values.First().ToString();

            Assert.AreEqual(string.Format("Пользователь {0} успешно зарегистрирован под ролью {1}",newUser.UserName,newUser.Role),msg);
        }

        [TestMethod]
        public void DeleteUser() {
            //Arrange (create template security db and check hes properties
            Mock<IAuthorizationEngage> mock = new Mock<IAuthorizationEngage>();

            mock.Setup(sm => sm.GetInfoLines).Returns(new[] {
                new SecurityInfoLine() {
                    UserId = 1,UserName = "Pavel",Email = "One@tut.by",IsConfirmed = true,RoleName = "Admin"
                },
                new SecurityInfoLine() {
                    UserId = 2,UserName = "Natali",Email = "Two@tut.by",IsConfirmed = false,RoleName = "Registar"
                },
                new SecurityInfoLine() {
                    UserId = 3,UserName = "Roman",Email = "Three@tut.by",IsConfirmed = true,RoleName = "Accountant"
                }
            });

            mock.Setup(m => m.DeleteUserById(1)).Verifiable();
            AccountController target = new AccountController(null);
            
            //Act
            ActionResult result =  target.DeleteUser(1);
            
            //Assert
            mock.Verify();
            Assert.IsInstanceOfType(result,typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void ChangePassword() {

        }

        [TestMethod]
        public void ChangeRole() { }
    }
}