using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using NaftanRailway.WebUI;
using NaftanRailway.WebUI.Areas.NomenclatureScroll;

namespace NaftanRailway.Test {
    /// <summary>
    /// Test of routes
    /// </summary>
    [TestClass]
    public class RouteTests {
        /// <summary>
        ///  I expose the UrL i want to test through the AppRelativeCurrentExecutionFilePath property of the HttpRequestBase class, 
        ///  and expose the HttpRequestBase through the Request property of the mock HttpContextBase class. 
        /// </summary>
        /// <param name="targetUrl"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        private HttpContextBase GreateHttpContext(string targetUrl = null, string httpMethod = "GET") {
            //create the mock request
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(targetUrl);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            //create the mock response
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            //create the mock context, sing the request and response
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResponse.Object);

            return mockContext.Object;
        }
        /// <summary>
        /// The parameters of this method let me specify the UrL to test, 
        /// the expected values for the controller and action segment variables, 
        /// and an object that contains the expected values for any additional variables i have defined.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="routeProperties"></param>
        /// <param name="httpMethod"></param>
        private void TestRouteMatch(string url, string controller, string action, object routeProperties = null, string httpMethod = "GET") {
            //Arrange
            RouteCollection routes = new RouteCollection();
            //RouteConfig.RegisterRoutes(routes);
           (new  NomenclatureScrollAreaRegistration()).RegisterArea(new AreaRegistrationContext("NomenclatureScroll",routes));

            //Act = process the route
            RouteData result = routes.GetRouteData(GreateHttpContext(url, httpMethod));

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(TestIncomingRouteResult(result, controller, action, routeProperties));
        }
        /// <summary>
        ///  To compare the result obtained from the routing system with the segment variable values i expect. 
        ///  This method uses .net reflection so that i can use an anonymous type to express any additional segment variables.
        /// </summary>
        /// <param name="routeResult"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="propertySet"></param>
        /// <returns></returns>
        private bool TestIncomingRouteResult(RouteData routeResult, string controller, string action, object propertySet = null) {
            Func<object, object, bool> valCompare = (v1, v2) => StringComparer.InvariantCultureIgnoreCase.Compare(v1, v2) == 0;

            bool result = valCompare(routeResult.Values["controller"], controller) && valCompare(routeResult.Values["action"], action);

            if(propertySet != null) {
                PropertyInfo[] propInfo = propertySet.GetType().GetProperties();
                if(propInfo.Any(pi => !(routeResult.Values.ContainsKey(pi.Name)) && valCompare(routeResult.Values[pi.Name], pi.GetValue(propertySet, null)))) {
                    return false;
                }
            }
            return result;
        }
        /// <summary>
        /// Check that a UrL does not work.
        /// </summary>
        /// <param name="url"></param>
        private void TestRouteFail(string url) {
            //Arrange
            RouteCollection routes = new RouteCollection();
            //RouteConfig.RegisterRoutes(routes);
            (new  NomenclatureScrollAreaRegistration()).RegisterArea(new AreaRegistrationContext("NomenclatureScroll",routes));

            //Act - process the route
            RouteData result = routes.GetRouteData(GreateHttpContext(url));

            Assert.IsTrue(result==null || result.Route==null);
        }

        [TestMethod]
        public void TestIncomingRoutes() {
            TestRouteMatch("~/Nomenclature","Scroll","Index",new {page = "1"});
            //constrains
            //TestRouteFail("~/Nomenclature/Scroll/ScrollDetails/3215/2016/2");
        }
    }
}