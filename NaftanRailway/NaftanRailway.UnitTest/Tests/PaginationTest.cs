using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.WebUI.Controllers;
using NaftanRailway.WebUI.HtmlHelpers;
using NaftanRailway.WebUI.Models;
using NaftanRailway.WebUI.ViewModels;


namespace NaftanRailway.UnitTest.Tests {
    [TestClass]
    public class PaginationTest {
        /// <summary>
        /// Check paggination processe
        /// </summary>
        [TestMethod]
        public void Can_Paginate() {
            //Arrange
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                new v_otpr {id = 1, n_otpr = "000000001",date_oper = DateTime.Today},
                new v_otpr {id = 2, n_otpr = "000000002",date_oper = DateTime.Today},
                new v_otpr {id = 3, n_otpr = "000000003",date_oper = DateTime.Today},
                new v_otpr {id = 4, n_otpr = "000000004",date_oper = DateTime.Today},
                new v_otpr {id = 5, n_otpr = "000000005",date_oper = DateTime.Today},
            }).AsQueryable());

            Ceh18Controller controller = new Ceh18Controller(mock.Object) { /*PageSize = 3*/ };
            SessionStorage storage = new SessionStorage();
            InputMenuViewModel input = new InputMenuViewModel();

            //Act
            DispatchListViewModel result = (DispatchListViewModel)controller.Index(storage, input, EnumOperationType.All, 2).Model;

            //Assert
            Shipping[] arrayOtprs = result.Dispatchs.ToArray();

            Assert.IsTrue(arrayOtprs.Length == 2);
            Assert.AreEqual(arrayOtprs[0].VOtpr.id, 4);
            Assert.AreEqual(arrayOtprs[1].VOtpr.n_otpr, "000000005");
        }

        /// <summary>
        /// Check generation of Html helper page links
        /// </summary>
        [TestMethod]
        public void Can_Generate_Page_Links() {
            // Arrange
            // define an HTML helper - we need to do this in order to apply the extension method
            HtmlHelper myHelper = null;

            // Arrange - create PagingInfo data
            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, 3, pageUrlDelegate);

            // Assert
            Assert.AreEqual(@"<a aria-label=""Previous"" class=""btn btn-default"" href=""Page1""><span aria-hidden=""true"">&laquo</span></a>"
                            + @"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>"
                            + @"<a aria-label=""Previous"" class=""btn btn-default"" href=""Page3""><span aria-hidden=""true"">&raquo</span></a>",
                            result.ToString());
        }

        /// <summary>
        /// Check Send View model with data list dispatch and pageInfo object
        /// </summary>
        [TestMethod]
        public void Can_Send_Pagination_View_Model() {
            //Arrange
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                new v_otpr(){id = 1,n_otpr = "00000001",date_oper = DateTime.Today},
                new v_otpr(){id = 2,n_otpr = "00000002",date_oper = DateTime.Today},
                new v_otpr(){id = 3,n_otpr = "00000003",date_oper = DateTime.Today},
                new v_otpr(){id = 4,n_otpr = "00000004",date_oper = DateTime.Today},
                new v_otpr(){id = 5,n_otpr = "00000005",date_oper = DateTime.Today},
            }).AsQueryable());

            Ceh18Controller controller = new Ceh18Controller(mock.Object) { /*PageSize = 3*/ };
            SessionStorage storage = new SessionStorage(DateTime.Today);
            InputMenuViewModel input = new InputMenuViewModel();
            //Act
            DispatchListViewModel result = (DispatchListViewModel)controller.Index(storage,input,EnumOperationType.All, 2).Model;

            //Assert
            PagingInfo pageInfo = result.PagingInfo;

            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        /// <summary>
        /// Check result dinamic pagination
        /// </summary>
        [TestMethod]
        public void Pagination_Dinamic_Calculation() {
            //Arrange (need for apply extension method) etc static class
            HtmlHelper myHelper = null;

            //Arrange (default PagingInfo object)
            PagingInfo pInfo = new PagingInfo() {
                CurrentPage = 8,
                TotalItems = 100,
                ItemsPerPage = 7,
            };

            //Arrange Size paging bar
            const int sizeBar = 7;

            //Arrange (Delegate for url building)
            Func<int, string> pageUrl = i => "Page" + i.ToString();

            //Act
            MvcHtmlString result = myHelper.PageLinks(pInfo, sizeBar, pageUrl);

            //Assert
            Assert.AreEqual(@"<a aria-label=""Previous"" class=""btn btn-default"" href=""Page7""><span aria-hidden=""true"">&laquo</span></a>"
                          + @"<a class=""btn btn-default"" href=""Page4"">4</a>"
                          + @"<a class=""btn btn-default"" href=""Page5"">5</a>"
                          + @"<a class=""btn btn-default"" href=""Page6"">6</a>"
                          + @"<a class=""btn btn-default"" href=""Page7"">7</a>"
                          + @"<a class=""btn btn-default btn-primary selected"" href=""Page8"">8</a>"
                          + @"<a class=""btn btn-default"" href=""Page9"">9</a>"
                          + @"<a class=""btn btn-default"" href=""Page10"">10</a>"
                          + @"<a class=""btn btn-default"" href=""Page11"">11</a>"
                          + @"<a aria-label=""Previous"" class=""btn btn-default"" href=""Page9""><span aria-hidden=""true"">&raquo</span></a>"
                , result.ToString());
        }

        /// <summary>
        /// testing that i am able to generate the current product count for different categories is simple. i create a mock 
        /// repository that contains known data in a range of categories and then call the List action method requesting 
        /// each category in turn. here is the unit test:
        /// </summary>
        [TestMethod]
        public void Generate_Category_Specific_Shipping_Count() {
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                new v_otpr() {id = 1, n_otpr = "00000001", oper = null,date_oper = DateTime.Today},
                new v_otpr() {id = 2, n_otpr = "00000002", oper = 1,date_oper = DateTime.Today},
                new v_otpr() {id = 3, n_otpr = "00000003", oper = 1,date_oper = DateTime.Today},
                new v_otpr() {id = 4, n_otpr = "00000004", oper = 1,date_oper = DateTime.Today},
                new v_otpr() {id = 5, n_otpr = "00000005", oper = 2,date_oper = DateTime.Today},
                new v_otpr() {id = 6, n_otpr = "00000006", oper = 1,date_oper = DateTime.Today},
                new v_otpr() {id = 7, n_otpr = "00000007", oper = 2,date_oper = DateTime.Today},
            }).AsQueryable());

            // Arrange - create a controller and make the page size 3 items
            Ceh18Controller target = new Ceh18Controller(mock.Object) { /*PageSize = 3*/ };
            SessionStorage storage = new SessionStorage();
            InputMenuViewModel input = new InputMenuViewModel();

            // Action - test the product counts for different categories
            int res1 = ((DispatchListViewModel)target.Index(storage, input, EnumOperationType.Sending).Model).PagingInfo.TotalItems;
            int res2 = ((DispatchListViewModel)target.Index(storage, input, EnumOperationType.Arrivals).Model).PagingInfo.TotalItems;
            //int res3 = ((DispatchListViewModel)target.Index(null,null).model).PagingInfo.TotalItems;

            // Assert
            Assert.AreEqual(4, res1);
            Assert.AreEqual(2, res2);
            //Assert.AreEqual(7, res3);
        }
    }
}
