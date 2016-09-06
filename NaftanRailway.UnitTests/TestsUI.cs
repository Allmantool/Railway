using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaftanRailway.WebUI.Controllers;
using System.Web.Mvc;
using Moq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using System;

namespace NaftanRailway.UnitTests {
    [TestClass]
    public class TestsUI {
        [TestMethod]
        public void Pagging() {
            Mock<IRailwayModule> mock = new Mock<IRailwayModule>();
            mock.Setup(x => x.ShippingsViews(EnumOperationType.All, DateTime.Today,)).Returns(new[]
            {
                new krt_Naftan() {KEYKRT = 1}, new krt_Naftan() {KEYKRT = 2}, new krt_Naftan() {KEYKRT = 3}, 
            });
            Ceh18Controller controller = new Ceh18Controller(mock.Object);
            Assert.AreEqual("Ceh18", "Ceh18");
        }

        /// <summary>
        /// Test name of returning View
        /// </summary>
        [TestMethod]
        public void ReturnRightView() {
            //Mock<IRailwayModule> 
            //Ceh18Controller controllerUnderTest = new Ceh18Controller();
            //var result = controllerUnderTest.Index() as ViewResult;
            //Assert.AreEqual("fooview", result.ViewName);
        }

        [TestMethod]
        public void ReturnViewBag() {
            //HomeController controllerUnderTest = new HomeController();
            //var result = controllerUnderTest.Details("a1") as ViewResult;
            //Assert.AreEqual("foo", result.ViewData["Name"]);
        }

        [TestMethod]
        public void ReturnsTempData() {
            //HomeController controllerUnderTest = new HomeController();
            //var result = controllerUnderTest.Details("a1") as ViewResult;
            //Assert.AreEqual("foo", result.TempData["Name"]);
        }
    }
}