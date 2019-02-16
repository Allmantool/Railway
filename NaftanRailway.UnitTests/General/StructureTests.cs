namespace NaftanRailway.UnitTests.General {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StructureTests {
        [TestMethod]
        public void DictTest() {
            var hierarchyDict = new Dictionary<int, string>{
                { 0, "Документ" },
                { 1, "Тип документа" },
                { 3, "Карточка" },
                { 7, "Перечень" },
                { 31, "Месяц" },
                { 63, "Год" },
            };

            //anonymous methods in a generalized and simple way
            Func<string, IEnumerable<int>> getTopTwo = (periodName) => hierarchyDict.OrderByDescending(x => x.Key)
                            .Where(x => x.Key <= hierarchyDict.FirstOrDefault(y => y.Value.Contains(periodName)).Key)
                            .Select(x => x.Key).Take(2);

            var sqlIn = string.Join(", ", getTopTwo.Invoke("Год"));
            Assert.AreEqual("63, 31", sqlIn);

            sqlIn = string.Join(", ", getTopTwo.Invoke("Тип документа"));
            Assert.AreEqual("1, 0", sqlIn);

            sqlIn = string.Join(", ", getTopTwo.Invoke("Документ"));
            Assert.AreEqual("0", sqlIn);
        }
    }
}