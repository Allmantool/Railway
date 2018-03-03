namespace NaftanRailway.UnitTests.Rail {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using BLL.POCO;
    using BLL.Services.ExpressionTreeExtensions;
    using Domain.Concrete.DbContexts.ORC;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LINQ {

        public class Person {

            public int Id { get; set; }

            public string Name { get; set; }

            public string Location { get; set; }

            public Person(int id, string name, string location) {
                Id = id;
                Name = name;
                Location = location;
            }
        };

        [TestMethod]
        public void FiltersDtoTest() {
            // Arrange
            var seq = new[] { "First", "Second", "Third" };

            var cards = new List<krt_Naftan_orc_sapod> {
                new krt_Naftan_orc_sapod() { id_kart = 11, nkrt = @"K_1" },
                new krt_Naftan_orc_sapod() { id_kart = 12, nkrt = @"K_2" },
                new krt_Naftan_orc_sapod() { id_kart = 13, nkrt = @"K_3" },
                new krt_Naftan_orc_sapod() { id_kart = 14, nkrt = @"K_4" },
                new krt_Naftan_orc_sapod() { id_kart = 15, nkrt = @"K_5" },
                new krt_Naftan_orc_sapod() { id_kart = 11, nkrt = @"K_1" },

                // dublicate 
                new krt_Naftan_orc_sapod() { id_kart = 12, nkrt = @"K_2" },
                new krt_Naftan_orc_sapod() { id_kart = 13, nkrt = @"K_3" },
                new krt_Naftan_orc_sapod() { id_kart = 14, nkrt = @"K_4" },

                // wrong cases (filter predicate)
                new krt_Naftan_orc_sapod() { id_kart = null, nkrt = @"K_null" },
            }.AsQueryable();

            Expression<Func<krt_Naftan_orc_sapod, object>> groupPredicate = x => new { x.id_kart, x.nkrt };
            Expression<Func<krt_Naftan_orc_sapod, bool>> filterPredicate = x => x.id_kart != null;

            // Act
            var result = cards.Where(filterPredicate)
                .OrderBy(x => x.nkrt)
                .GroupBy(groupPredicate)
                .ToDictionary(x => x.First().id_kart.Value.ToString(), x => x.First().nkrt);

            var availableValues = result.Values.Select(x => x);
            var availableKeys = result.Keys.Select(x => x);

            var filterDict = new CheckListFilter(result) {
                FieldName = PredicateExtensions.GetPropName<krt_Naftan_orc_sapod>(x => x.id_kart),
                NameDescription = @"Накоп. Карточки"
            };

            // There're four unique values
            var inputSeq = new[] { "1", "2", "3", "1" };
            // It moves them in dictinary with unique Key/Value 
            var inputDicionary = inputSeq.GroupBy(x => x).ToDictionary(x => x.First(), x => x.First());
            var dictValues = inputDicionary.Values.Select(x => x);

            // constructor with enum
            var filterEnum = new CheckListFilter(inputSeq);

            var expTree1 = filterDict.FilterByField<krt_Naftan_orc_sapod>();

            Assert.AreEqual(5, result.Count());
            Assert.AreEqual(5, availableValues.ToList().Count());
            Assert.AreEqual(@"Накоп. Карточки", filterDict.NameDescription);
            Assert.AreEqual(@"id_kart", filterDict.FieldName);
            Assert.AreEqual(5, filterDict.AllAvailableValues.Count());
            Assert.AreEqual(3, dictValues.Count());
            Assert.AreEqual(3, filterEnum.AllAvailableValues.Count());
        }
    }
}