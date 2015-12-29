using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.WebUI.Controllers;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.UnitTest.Tests {
    [TestClass]
    public class NavigationTests {
        [TestMethod]
        public void Can_Create_Categories() {
            // Arrange - create the mock repository
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            //*null-3,1-3,2-4
            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                    new v_otpr{id = 1,n_otpr = "00000001",oper = null},
                    new v_otpr{id = 2,n_otpr = "00000002",oper = 2},
                    new v_otpr{id = 3,n_otpr = "00000003",oper = 2},
                    new v_otpr{id = 4,n_otpr = "00000004",oper = 1},
                    new v_otpr{id = 5,n_otpr = "00000005",oper = null},
                    new v_otpr{id = 6,n_otpr = "00000006",oper = 2},
                    new v_otpr{id = 7,n_otpr = "00000007",oper = 1},
                    new v_otpr{id = 8,n_otpr = "00000008",oper = 2},
                    new v_otpr{id = 9,n_otpr = "00000009",oper = 1},
                    new v_otpr{id = 10,n_otpr = "000000010",oper = null},
            }).AsQueryable());

            // Arrange - create the controller
            NavigationController target = new NavigationController(mock.Object);
            InputMenuViewModel input = new InputMenuViewModel();

            // Act = get the set of categories
            short?[] results = ((IQueryable<short?>)target.MenuTypeOperations(input).ViewBag.TypeOperation).ToArray();

            // Assert
            Assert.AreEqual(3, (Int16)results.Length);

            if (results[0] != null)
                Assert.AreEqual(null, (Int16)results[0]);
            else
                Assert.AreEqual(null, null);

            var s = results[1];
            if (s != null) Assert.AreEqual(1, (Int16)s);
            var s1 = results[2];
            if (s1 != null) Assert.AreEqual(2, (Int16)s1);
        }
        /// <summary>
        /// i can test that the Menu action method correctly adds details of the selected category by reading the value of the 
        /// ViewBag property in a unit test, which is available through the ViewResult class. here is the test:
        /// </summary>
        [TestMethod]
        public void Indicates_Selected_Category() {
            // Arrange - create the mock repository
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
              new v_otpr() {id = 1, n_otpr = "00000001", oper = 1},
              new v_otpr() {id = 4, n_otpr = "00000004", oper = 2},
              new v_otpr() {id = 5, n_otpr = "00000005", oper = null},
              new v_otpr() {id = 8, n_otpr = "00000008", oper = 2},
              new v_otpr() {id = 12, n_otpr = "00000012", oper = 2},
            }).AsQueryable());

            // Arrange - create the controller
            NavigationController target = new NavigationController(mock.Object);

            // Arrange - define the category to selected
            const string categoryToSelect = "Sending";

            // Action
            string result = target.MenuTypeOperations(null, categoryToSelect).ViewBag.SelectedCategory;

            // Assert
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Date_Numbers_Menu() {
            //Arrange
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();
            NavigationController target = new NavigationController(mock.Object);
            SessionStorage storage = new SessionStorage();

            //Act
            InputMenuViewModel result = (InputMenuViewModel)target.GeneralMenu(storage, new InputMenuViewModel() {
                ReportPeriod = DateTime.Today
            }).ViewData.Model;

            //Assert
            Assert.AreEqual(DateTime.Today, result.ReportPeriod);

        }
    }
}
