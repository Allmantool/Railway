using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace NaftanRailway.UnitTests.General
{
    [TestClass]
    public class Linq
    {
        [TestMethod]
        public void Xml()
        {
            //XElement root = XElement.Load("PurchaseOrders.xml");
            XElement xdoc = XElement.Parse(@"
                <Root>  
                  <Child1>  
                    <GrandChild1>GC1 Value</GrandChild1>  
                    <GrandChild2>GC2 Value</GrandChild2>
                  </Child1>  
                  <Child2>  
                    <GrandChild2>GC2 Value</GrandChild2>  
                  </Child2>  
                  <Child3>  
                    <GrandChild3>GC3 Value</GrandChild3>  
                  </Child3>  
                  <Child4>  
                    <GrandChild4>GC4 Value</GrandChild4>  
                  </Child4>  
                </Root>");

            var dests = xdoc.Descendants("Child1");
            Assert.AreEqual(dests.Count(), 2);
        }
    }
}
