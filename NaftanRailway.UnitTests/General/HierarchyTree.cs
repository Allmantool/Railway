using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaftanRailway.BLL.DTO.General;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace NaftanRailway.UnitTests.General {
    /// <summary>
    /// Custom tree
    /// </summary>
    public class TreeNode : TreeNodeBase<TreeNode> {
        public TreeNode(string name) : base(name) {
            Debug.Write(name);
        }

        protected override TreeNode MySelf => this;
        public string Label { get; set; }
        public string SearchKey { get; set; }
        public int ChCount { get; set; }
    }


    [TestClass]
    public class HierarchyTree {
        [TestMethod]
        public void LinqHierarchy() {
            var data = (new[]
            {
                new krt_Naftan_orc_sapod() {keykrt = 1, id_kart = 1, tdoc = 1},
                new krt_Naftan_orc_sapod() {keykrt = 1, id_kart = 2, tdoc = 1},
                new krt_Naftan_orc_sapod() {keykrt = 2, id_kart = 3, tdoc = 1},
                new krt_Naftan_orc_sapod() {keykrt = 2, id_kart = 3, tdoc = 2},
            }).AsQueryable();

            //it stats work at the deepest level first and work up
            var groups = data.GroupBy(x => new { x.keykrt, x.id_kart, x.tdoc })
                             .GroupBy(x => new { x.Key.keykrt, x.Key.id_kart })
                             .GroupBy(x => new { x.Key.keykrt });

            Debug.WriteLine(groups.First().Key);              // will be an A value
            Debug.WriteLine(groups.First().First().First()); // will be an A, B, C group
            //Assert.IsTrue(;
        }

        [TestMethod]
        public void SetUp() {
            const int countGroup = 3;
            var startPeriod = new DateTime(2015, 1, 1);
            var serverName = "LOCALMACHINE";//"CPN8\HOMESERVER"

            var hirearchyDict = new Dictionary<int, string>{
                { 0, "Документ" },
                { 1, "Тип документа" },
                { 3, "Карточка" },
                { 7, "Перечень" },
                { 15, "Сбор" }
            };

            var conectString = ConfigurationManager.AppSettings["TestLocalConnection"] ??
                $@"data source = {serverName}; initial catalog = Railway; integrated security = True; Trusted_Connection = Yes;";


            #region Query
            /* 04.08.2017
            * It query converts flatted table to hierarchy table (The hierarchy deep is defined by group predicate)
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
                SELECT  [elementId] = ROW_NUMBER() OVER(ORDER BY kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [groupId] = DENSE_RANK() OVER(ORDER BY kn.KEYKRT DESC),
                    [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [treeLevel] = GROUPING_ID(kn.KEYKRT, kn.NKRT, knos.id_kart, knos.tdoc, knos.nomot),
                    --[level_card] = GROUPING(knos.id_kart),
                    [scroll] = kn.NKRT, kn.KEYKRT,
                    [card] = knos.NKRT, knos.id_kart,
		            [typeDoc] = knos.tdoc,
		            [docum] = knos.nomot,
                    [count] = COUNT(*)
                FROM [unCharge] AS knos INNER JOIN [listOfCards] AS kn
                    ON kn.KEYKRT = knos.keykrt
                WHERE knos.tdoc > 0 AND knos.id is not null AND kn.DTBUHOTCHET >= '{startPeriod:d}'
                GROUP BY GROUPING SETS(
                        --(),
                        (kn.KEYKRT, kn.NKRT),
		                --(kn.KEYKRT, kn.NKRT, knos.id_kart),
                        (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt),
		                (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc),
		                (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc, knos.nomot)
	                )
                )

                SELECT
                    [parentId] = CASE [treeLevel]
					    WHEN 0 THEN MAX(elementId) OVER (PARTITION BY KEYKRT, id_kart, typeDoc)
					    WHEN 1 THEN MAX(elementId) OVER (PARTITION BY KEYKRT, id_kart)
					    WHEN 3 THEN MAX(elementId) OVER (PARTITION BY KEYKRT)
					ELSE 0 END,
	                [elementId], [groupId], [rankInGr], [treeLevel],
	                [levelName] = CASE [treeLevel]
					    WHEN 0 THEN N'Документ'
					    WHEN 1 THEN N'Тип документа'
					    WHEN 3 THEN N'Карточка'
					    WHEN 7 THEN N'Перечень'
					ELSE NULL END,
	                [searchkey] = CASE [treeLevel]
					    WHEN 0 THEN convert(nvarchar(max), [docum])
					    WHEN 1 THEN convert(nvarchar(max), [typeDoc])
					    WHEN 3 THEN convert(nvarchar(max), [id_kart])
					    WHEN 7 THEN convert(nvarchar(max), [keykrt])
					ELSE NULL END,
	                [label] = CASE [treeLevel]
				        WHEN 0 THEN Convert(nvarchar(max), [docum])
				        WHEN 1 THEN Case [typeDoc] when 1 then N'Накладная' when 2 then N'Ведомость' when 3 then N'Акт' Else N'Карточка' End
				        WHEN 3 THEN [card]
				        WHEN 7 THEN CONVERT(NVARCHAR(10),[scroll])
				    ELSE NULL END,
	                [count]
                FROM grSubResult as gr
                WHERE [groupId] <= {countGroup} --  and [treeLevel] IN ( 1, 0)
                ORDER BY KEYKRT DESC, id_kart desc, gr.[typeDoc] desc, [docum] desc;";
            #endregion

            IList<HierarchyDto> result = new List<HierarchyDto>();
            IList<TreeNode> tree = new List<TreeNode>();

            //Act
            using (var dt = new DataTable()) {
                using (var con = new SqlConnection(conectString))
                using (var adpt = new SqlDataAdapter(query, con)) {
                    try {
                        adpt.Fill(dt);
                    } catch (SqlException ex) {
                        if (con.State == ConnectionState.Open) con.Close();
                        Debug.WriteLine($"Test throws exception: {ex.Message}");
                    } finally {
                        if (con.State == ConnectionState.Open) con.Close();
                        adpt.Dispose();
                    }
                }

                //Arrange
                Assert.IsTrue(dt.Rows.Count > 0);

                if (dt.Rows.Count > 0) {
                    result = dt.Select().ToList().Select(x => new HierarchyDto{
                        Id = Convert.ToInt32(x["elementId"]),
                        Label = x["label"].ToString(),
                        SearchKey = x["searchkey"].ToString(),
                        ParentId = Convert.ToInt32(x["parentId"]),
                        LevelName = x["levelName"].ToString(),
                        ChCount = Convert.ToInt32(x["count"])
                    }).ToList();
                }
            }

            //fill tree
            try {
                tree = FillRecursive(result.ToList());
            } catch (Exception ex) {
                Debug.WriteLine($"Test throws exception: {ex.Message}");
            }

            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(tree.Count() == countGroup);
        }

        private class HierarchyDto {
            public int Id { get; set; }
            public string Label { get; set; }
            public int? ParentId { get; set; }
            public string SearchKey { get; set; }
            public string LevelName { get; set; }
            public int ChCount { get; set; }
        }

        /// <summary>
        /// It fills tree recursive (up to down)
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static List<TreeNode> FillRecursive(IList<HierarchyDto> rows, int parentId = 0) {
            var subResult = rows.Where(x => x.ParentId.Equals(parentId));

            var result = subResult.Select(item => new TreeNode(item.LevelName) {
                Id = item.Id,
                Children = FillRecursive(rows, item.Id),
                Label = item.Label,
                SearchKey = item.SearchKey,
                ChCount = item.ChCount
            }).ToList();

            return result;
        }

    }
}