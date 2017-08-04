using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaftanRailway.BLL.DTO.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace NaftanRailway.UnitTests.General {

    public class TreeNode : TreeNodeBase<TreeNode>, ITreeNode<TreeNode> {
        public TreeNode(string name) : base(name) {
            Debug.Write(name);
        }

        protected override TreeNode MySelf {
            get {
                return this;
            }
        }
    }

    [TestClass]
    public class HierarchyTree {
        [TestMethod]
        public void SetUp() {
            const int countGroup = 3;
            var startPeriod = new DateTime(2017, 1, 1);

            var hirearchyDict = new Dictionary<int, string>{
                { 0, "Карточка" },
                { 1, "Перечень" }
            };

            var conectString = ConfigurationManager.AppSettings["TestLocalConnection"] ??
                @"data source = CPN8\HOMESERVER; initial catalog = Railway; integrated security = True; Trusted_Connection = Yes;";


            #region Query 
            /* 04.08.2017
            * It query converts flatted table to hierachy table (The hierachy deep is defined by group predicate)
            * 
            * [elementId] - primary key
            * [parentId] - parents element id
            * [groupId] - group id
            * [rankInGr] - element primary key in group
            * [treeLevel] - height of tree
            * [levelName] - custom group tree node name
            * [searchkey] - key for search in plane (source table)
            * [label] - description for rendering purpose
            */
            //--Warning weakness!
            //--The order must be same in each aggregation functions
            var query = $@";WITH grSubResult AS (
                SELECT  [elementId] = ROW_NUMBER() OVER(ORDER BY kn.KEYKRT DESC, knos.id_kart desc),
                        [groupId] = DENSE_RANK() OVER(ORDER BY kn.KEYKRT DESC),
                        [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY kn.KEYKRT DESC, knos.id_kart desc),
                        [treeLevel] = GROUPING_ID(kn.KEYKRT, kn.NKRT, knos.id_kart),
                        --[level_card] = GROUPING(knos.id_kart),
                        [scroll] = kn.NKRT, kn.KEYKRT,
                        [card] = knos.NKRT, knos.id_kart,
                        [count] = COUNT(*)
                FROM [unCharge] AS knos INNER JOIN [listOfCards] AS kn
                    ON kn.KEYKRT = knos.keykrt
                WHERE knos.tdoc > 0 AND kn.DTBUHOTCHET >= '{startPeriod.ToString("d")}'
                GROUP BY GROUPING SETS(
                        --(),
                        (kn.KEYKRT, kn.NKRT),
                        (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt)
	                )
                )

                SELECT 
                    [parentId] = CASE [elementId] when MAX(elementId) OVER (PARTITION BY KEYKRT) THEN 0 ELSE MAX(elementId) OVER (PARTITION BY KEYKRT) END,
	                [elementId], [groupId], [rankInGr], [treeLevel],
	                [levelName] = CASE [treeLevel] WHEN 0 THEN N'{hirearchyDict[0]}' WHEN 1 THEN N'{hirearchyDict[1]}' ELSE NULL END,
	                [searchkey] = CASE [treeLevel] WHEN 0 THEN [id_kart] WHEN 1 THEN [keykrt] ELSE NULL END,
	                [label] = CASE [treeLevel] WHEN 0 THEN [card] WHEN 1 THEN CONVERT(NVARCHAR(max),[scroll]) ELSE NULL END
                FROM grSubResult
                WHERE [treeLevel] IN ( 1, 0) and [groupId] <= {countGroup}
                ORDER BY KEYKRT DESC, id_kart desc;";
            #endregion

            IList<Category> result = null;
            IList<TreeNode> tree = null;

            //Act
            using (var dt = new DataTable()) {
                using (var con = new SqlConnection(conectString))
                using (var Adpt = new SqlDataAdapter(query, con)) {
                    try {
                        Adpt.Fill(dt);
                    } catch (SqlException ex) {
                        if (con.State == ConnectionState.Open) con.Close();
                        Debug.WriteLine($"Test throws exception: {ex.Message}");
                    } finally {
                        if (con != null)
                            if (con.State == ConnectionState.Open) con.Close();
                        Adpt.Dispose();
                    }
                }

                //Arrange
                Assert.IsTrue(dt.Rows.Count > 0);

                if (dt != null && dt.Rows.Count > 0) {
                    result = dt.Select().ToList().Select(x => new Category {
                        Id = Convert.ToInt32(x["elementId"]),
                        CategoryName = x["levelName"].ToString(),
                        Label = x["label"].ToString(),
                        searchKey = Convert.ToInt64(x["searchkey"]),
                        ParentId = Convert.ToInt32(x["parentId"]),
                    }).ToList();
                }
            }

            //fill tree
            try {
                tree = FillRecursive(result.ToList());
            } catch (Exception) {
                throw;
            }

            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(tree.Count() == countGroup);
        }

        public class Category {
            public int Id { get; set; }
            public string CategoryName { get; set; }
            public string Label { get; set; }
            public int? ParentId { get; set; }
            public long searchKey { get; set; }
        }

        private static List<TreeNode> FillRecursive(List<Category> rows, int parentId = 0) {

            var subResult = rows.Where(x => x.ParentId.Equals(parentId));

            var result = subResult.Select(item => new TreeNode(item.CategoryName) {
                Id = item.Id,
                Children = FillRecursive(rows, item.Id)
            }).ToList();

            return result;
        }

    }
}