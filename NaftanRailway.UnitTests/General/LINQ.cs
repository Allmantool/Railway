namespace NaftanRailway.UnitTests.General
{
    using System.Linq;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Linq
    {
        [TestMethod]
        public void Xml()
        {
            // XElement root = XElement.Load("PurchaseOrders.xml");
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

        [TestMethod]
        public void Delete_IEnumerable()
        {
            // Arrange.
            var enumerableCollection = Enumerable.Range(1, 10);

            // Act.
            var collection = enumerableCollection.ToList();
            var countAtStart = collection.ToList().Count;

            enumerableCollection = collection.Where( i => i != 1);

            var countAtEnd = enumerableCollection.ToList().Count;

            // Assert.
            countAtEnd.Should().BeLessThan(countAtStart);
        }
    }
}