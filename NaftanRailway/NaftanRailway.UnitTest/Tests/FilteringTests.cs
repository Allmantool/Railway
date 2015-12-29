using System;
using System.Linq;
//using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.WebUI.Controllers;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.UnitTest.Tests {
    [TestClass]
    public class FilteringTests {
        /// <summary>
        /// Check filter by category result
        /// </summary>
        [TestMethod]
        public void FilterCategoryOperations() {
            //Arrange (Greate mock repository)
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            //*null-3,1-3,2-4
            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                new v_otpr{id = 1,n_otpr = "00000001",oper = null,date_oper = DateTime.Today},
                new v_otpr{id = 2,n_otpr = "00000002",oper = 2,date_oper = DateTime.Today},
                new v_otpr{id = 3,n_otpr = "00000003",oper = 2,date_oper = DateTime.Today},
                new v_otpr{id = 4,n_otpr = "00000004",oper = 1,date_oper = DateTime.Today},
                new v_otpr{id = 5,n_otpr = "00000005",oper = null,date_oper = DateTime.Today},
                new v_otpr{id = 6,n_otpr = "00000006",oper = 2,date_oper = DateTime.Today},
                new v_otpr{id = 7,n_otpr = "00000007",oper = 1,date_oper = DateTime.Today},
                new v_otpr{id = 8,n_otpr = "00000008",oper = 2,date_oper = DateTime.Today},
                new v_otpr{id = 9,n_otpr = "00000009",oper = 1,date_oper = DateTime.Today},
                new v_otpr{id = 10,n_otpr = "000000010",oper = null,date_oper = DateTime.Today},
            }).AsQueryable());

            //Arrange (Greate controller)
            Ceh18Controller controller = new Ceh18Controller(mock.Object) { /*PageSize = 3*/ };

            InputMenuViewModel input = new InputMenuViewModel();
            SessionStorage storage = new SessionStorage();

            //Act
            Shipping[] result = ((DispatchListViewModel)controller.Index(storage, input, EnumOperationType.Arrivals).Model).Dispatchs.ToArray();

            //Assert
            Assert.AreEqual(3, result.Length);
            Assert.IsTrue(result[0].VOtpr.n_otpr == "00000002" && result[0].VOtpr.oper == 2);
            Assert.IsTrue(result[1].VOtpr.n_otpr == "00000003" && result[1].VOtpr.oper == 2);
        }
        /// <summary>
        /// Check filter by period
        /// </summary>
        [TestMethod]
        public void FilterByDate() {
            //Arrange
            DateTime? inputData1 = new DateTime(2015, 01, 26);
            DateTime? inputData2 = new DateTime(2015, 02, 03);

            //act
            DateTime startDate1 = new DateTime(inputData1.Value.Year, inputData1.Value.Month, 1);
            DateTime endDate1 = startDate1.AddMonths(1).AddDays(-1);

            DateTime startDate2 = new DateTime(inputData2.Value.Year, inputData2.Value.Month, 1);

            //Assert
            Assert.AreEqual(new DateTime(2015, 01, 31), endDate1);
            Assert.AreEqual(new DateTime(2015, 02, 01), startDate2);
        }
        /// <summary>
        /// Searched number
        /// </summary>
        [TestMethod]
        public void FilterShippingNumber() {
            //Arrange
            const string shNumber1 = "78979446";
            const string shNumber2 = "01546489";

            //string searchPattern = null;
            //Act

            //Assert
            Assert.IsTrue(shNumber1.StartsWith("78"));
            Assert.IsTrue(shNumber2.StartsWith(""));
            Assert.IsTrue(shNumber2.StartsWith(""));
        }

        [TestMethod]
        public void Autocomplete() {
            //Arrange
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                new v_otpr{id = 1,n_otpr = "00000001",oper = null,date_oper = DateTime.Today},
                new v_otpr{id = 2,n_otpr = "02000002",oper = 2,date_oper = DateTime.Today},
                new v_otpr{id = 5,n_otpr = "02000005",oper = null,date_oper = DateTime.Today},
                new v_otpr{id = 6,n_otpr = "00000006",oper = 2,date_oper = DateTime.Today},
                new v_otpr{id = 7,n_otpr = "02000007",oper = 1,date_oper = DateTime.Today},
                new v_otpr{id = 8,n_otpr = "00000008",oper = 2,date_oper = DateTime.Today},
            }).AsQueryable());

            Ceh18Controller controller = new Ceh18Controller(mock.Object);

            InputMenuViewModel input = new InputMenuViewModel() { ReportPeriod = DateTime.Today, ShippingChoise = "02" };
            SessionStorage storage = new SessionStorage();

            //Act
            Shipping[] result = ((DispatchListViewModel)controller.Index(storage, input, EnumOperationType.All).Model).Dispatchs.ToArray();

            //Assert
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void GenerateFilterDate() {
            //Arrange
            DateTime chooseDate, startFilterDate, endFilterDate;

            //act
            chooseDate = new DateTime(2015, 8, 1);
            startFilterDate = chooseDate.AddDays(-5);
            endFilterDate = chooseDate.AddMonths(1).AddDays(5);

            //Assert
            Assert.AreEqual(new DateTime(2015, 7, 27), startFilterDate);
            Assert.AreEqual(new DateTime(2015, 9, 6), endFilterDate);
        }
    }
}
