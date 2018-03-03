namespace NaftanRailway.UnitTests.General
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using BLL.DTO.General;
    using Domain.Concrete.DbContexts.ORC;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MoreLinq;

    /// <summary>
    /// Custom tree
    /// </summary>
    public class TreeNode : TreeNodeBase<TreeNode>
    {

        public TreeNode()
        {
        }

        public TreeNode(string name) : base(name)
        {
            Debug.Write(name);
        }

        protected override TreeNode MySelf => this;

        [Key()/*, Column("id")*/]
        public override long Id { get; set; }

        public long ParentId { get; set; }

        public int GroupId { get; set; }

        public int RankInGr { get; set; }

        public short TreeLevel { get; set; }

        public string Label { get; set; }

        public string SearchKey { get; set; }

        public long Count { get; set; }

        public byte[] RootKey { get; set; }

        public string StrKey { get; set; }

        /// <summary>
        /// Return byte array instead of base64String()
        /// </summary>
        public int[] ByteArray { get { return RootKey.Select(b => (int)b).ToArray(); } }
    }

    public partial class NomenclatureEntities : DbContext
    {
        public NomenclatureEntities()
            : base("name=EFConnection")
        {
            // var dbCon = new DbConnection();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<krt_Naftan_orc_sapod> Charges { get; set; }

        public virtual DbSet<krt_Naftan> Scrolls { get; set; }
    }

    [TestClass]
    public class HierarchyTree
    {
        [TestMethod]
        public void LinqHierarchy()
        {
            var data = new[]
            {
                new krt_Naftan_orc_sapod() {keykrt = 1, id_kart = 1, tdoc = 1},
                new krt_Naftan_orc_sapod() {keykrt = 1, id_kart = 2, tdoc = 1},
                new krt_Naftan_orc_sapod() {keykrt = 2, id_kart = 3, tdoc = 1},
                new krt_Naftan_orc_sapod() {keykrt = 2, id_kart = 3, tdoc = 2},
            }.AsQueryable();

            var groups = data.GroupBy(x => new { x.keykrt, x.id_kart, x.tdoc })
                             .GroupBy(x => new { x.Key.keykrt, x.Key.id_kart })
                             .GroupBy(x => new { x.Key.keykrt });

            Debug.WriteLine(groups.First().Key);              // will be an A value
            Debug.WriteLine(groups.First().First().First()); // will be an A, B, C group

            // Assert.IsTrue(;
        }

        [TestMethod]
        public void SetUp()
        {
            const int countGroup = 20;
            var startPeriod = new DateTime(2017, 1, 1);
            var serverName = @"DB2"; // @"CPN8\HOMESERVER";//"LOCALMACHINE";
            var dbName = @"NSD2";

            var hirearchyDict = new Dictionary<int, string>{
                { 0, "Документ" },
                { 1, "Тип документа" },
                { 3, "Карточка" },
                { 7, "Перечень" },
                { 15, "Сбор" }
            };

            var typeDocDict = new Dictionary<int, string> {
                { 1, "Накладная" },
                { 2, "Ведомость" },
                { 3, "Акт" },
                { 4, "Карточка" },
            };

            var conectString = ConfigurationManager.AppSettings["TestLocalConnection"] ??
                $@"data source={serverName};initial catalog={dbName};integrated security=True;Trusted_Connection=Yes;";

            #region Query
            /* 04.08.2017
            * It query converts flatted table to hierarchy table (The hierarchy deep is defined by group predicate)
            *
            * [id] - primary key
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
                SELECT  
                    [id] = ROW_NUMBER() OVER(ORDER BY kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [groupId] = DENSE_RANK() OVER(ORDER BY kn.KEYKRT DESC),
                    [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [treeLevel] = GROUPING_ID(kn.KEYKRT, kn.NKRT, knos.id_kart, knos.tdoc, knos.nomot),
                    --[level_card] = GROUPING(knos.id_kart),
                    [scroll] = kn.NKRT, kn.KEYKRT,
                    [card] = knos.NKRT, knos.id_kart,
		            [typeDoc] = knos.tdoc,
		            [docum] = knos.nomot,
                    [count] = COUNT(*)
                FROM [dbo].[krt_Naftan_orc_sapod] AS knos INNER JOIN [dbo].[krt_Naftan] AS kn
                    ON kn.KEYKRT = knos.keykrt
                WHERE knos.tdoc > 0 AND knos.id > 0 AND kn.DTBUHOTCHET >= '{startPeriod:d}'
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
					    WHEN 0 THEN MAX(id) OVER (PARTITION BY KEYKRT, id_kart, typeDoc)
					    WHEN 1 THEN MAX(id) OVER (PARTITION BY KEYKRT, id_kart)
					    WHEN 3 THEN MAX(id) OVER (PARTITION BY KEYKRT)
					ELSE 0 END,
	                [id], [groupId], [rankInGr], [treeLevel],
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

            IList<DataRow> result = new List<DataRow>();
            IList<TreeNode> tree = new List<TreeNode>();

            //Act
            using (var dt = new DataTable())
            {
                using (var con = new SqlConnection(conectString))
                using (var adpt = new SqlDataAdapter(query, con))
                {
                    try
                    {
                        adpt.Fill(dt);
                        result = dt.Select().ToList();

                        // fill tree
                        if (dt.Rows.Count > 0) tree = result.FillRecursive();
                    }
                    catch (SqlException ex)
                    {
                        Debug.WriteLine($"Test throws exception: {ex.Message}");
                        if (con.State == ConnectionState.Open) con.Close();
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open) con.Close(); adpt.Dispose();
                    }
                }
            }

            var totalCardCount = tree.Sum(x => x.Children.Count());
            var totalDocumCount = tree.Sum(x => x.Descendants().Count(node => node.LevelName == hirearchyDict[0]));
            var totalActCount = tree.Sum(x => x.Descendants().Where(node => node.LevelName.Equals(hirearchyDict[1]) && node.Label.Equals(typeDocDict[3])).Select(act => act.Count).Count());
            var documStartsWith = tree.SelectMany(x => x.Descendants()).Where(node => node.Label.StartsWith("01")).ToList();
            var documDictFilter = tree.SelectMany(x => x.Descendants()).Where(node => node.LevelName.Equals(hirearchyDict[0]))
                                     .DistinctBy(x => new { x.SearchKey, x.Label }).ToDictionary(gr => gr.SearchKey, gr => gr.Label);
            var typeDocDictFilter = tree.SelectMany(x => x.Descendants()).Where(node => node.LevelName.Equals(hirearchyDict[1]))
                                    .DistinctBy(x => new { x.SearchKey, x.Label }).ToDictionary(gr => gr.SearchKey, gr => gr.Label);

            var cardDictFilter = tree.SelectMany(x => x.Descendants()).Where(node => node.LevelName.Equals(hirearchyDict[3]))
                                     .DistinctBy(x => new { x.SearchKey, x.Label })//.ToLookup(x=>x.SearchKey)
                                     .ToDictionary(gr => gr.SearchKey, gr => gr.Label, StringComparer.OrdinalIgnoreCase);

            // Arrange
            Assert.IsTrue(totalCardCount > 0);
            Assert.IsTrue(totalDocumCount > totalCardCount);
            Assert.IsTrue(totalActCount > 0 && totalActCount < totalCardCount);
            Assert.IsTrue(documStartsWith.Count() >= 0);
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(tree.Count() == countGroup);
        }

        [TestMethod]
        public void InitEf()
        {
            byte[] rootKey = null; //Encoding.ASCII.GetBytes("0xE2E7E8B3878D0B7897E01E049C5CD89B");//Byte.Parse("0xE2E7E8B3878D0B7897E01E049C5CD89B");
            var typeDoc = string.Join(", ", new[] { 63 });
            var startPeriod = new DateTime(2017, 1, 1);

            #region Query

            /* 04.08.2017
            * It query converts flatted table to hierarchy table (The hierarchy deep is defined by group predicate)
            *
            * [id] - primary key
            * [parentId] - parents element id
            * [groupId] - group id
            * [rankInGr] - element primary key in group
            * [treeLevel] - height of tree
            * [levelName] - custom group tree node name
            * [searchkey] - key for search in plane (source table)
            * [label] - description for rendering purpose
            * [rootKey] - hierarchyid base on hashbyte
            */

            // --Warning weakness!
            // --The order must be same in each aggregation functions
            var query = $@"
            Declare @tree TABLE(
	            [parentId] BIGINT,		[id] BIGINT,				[groupId] INT,				[rankInGr] INT,
	            [treeLevel] SMALLINT,	[levelName] NVARCHAR(30),   [searchkey] NVARCHAR(30),	[label] NVARCHAR(30),
	            [count] BIGINT,			[rootKey] varbinary(1000) primary key,                  [strKey] NVARCHAR(MAX)
            );

            ;WITH grSubResult AS (
                SELECT
                    [id] = ROW_NUMBER() OVER(ORDER BY YEAR(DTBUHOTCHET) DESC, MONTH(kn.DTBUHOTCHET) DESC, kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [groupId] = DENSE_RANK() OVER(ORDER BY YEAR(DTBUHOTCHET) DESC, MONTH(kn.DTBUHOTCHET) DESC, kn.KEYKRT DESC),
                    [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY YEAR(DTBUHOTCHET) DESC, MONTH(kn.DTBUHOTCHET) DESC, kn.KEYKRT DESC, knos.id_kart desc, knos.tdoc desc, knos.nomot desc),
                    [treeLevel] = GROUPING_ID( YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.tdoc, knos.nomot),
                    --[level_card] = GROUPING(knos.id_kart),
                    [period] =  CONVERT(DATE, N'01.' + CONVERT(NVARCHAR(2),MONTH(kn.DTBUHOTCHET)) + N'.' + CONVERT(NVARCHAR(4),YEAR(kn.DTBUHOTCHET)) ),
                    [year] = YEAR(kn.DTBUHOTCHET),
                    [month] = MONTH(kn.DTBUHOTCHET),
                    [scroll] = kn.NKRT, kn.KEYKRT,
                    [card] = knos.NKRT, knos.id_kart,
		            [typeDoc] = knos.tdoc,
		            [docum] = knos.nomot,
                    [count] = COUNT(*),
                    [rootKey] = HASHBYTES('md5',
			            ISNULL(Convert(nvarchar(4),YEAR(DTBUHOTCHET)),N'') +
			            ISNULL(N'-->' + Convert(nvarchar(2),MONTH(kn.DTBUHOTCHET)),N'') +
			            ISNULL(N'-->' + convert(nvarchar(20), kn.KEYKRT), N'') +
			            ISNULL(N'-->' + convert(nvarchar(10), kn.NKRT), N'') +
			            ISNULL(N'-->' + Convert(nvarchar(10),knos.id_kart),N'') +
			            ISNULL(N'-->' + Convert(nvarchar(10),knos.nkrt),N'') +
			            ISNULL(N'-->' + Convert(nvarchar(1), knos.tdoc), N'') +
			            ISNULL(N'-->' + Convert(nvarchar(10),knos.nomot), N''))
                FROM [dbo].[krt_Naftan_orc_sapod] AS knos INNER JOIN [dbo].[krt_Naftan] AS kn
                    ON kn.KEYKRT = knos.keykrt
                WHERE knos.tdoc > 0 AND knos.id > 0
                GROUP BY GROUPING SETS(
                       --(),
                        (YEAR(DTBUHOTCHET)),
                        (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET)),
                        (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT),
		                --(kn.KEYKRT, kn.NKRT, knos.id_kart),
                        (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt),
		                (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc),
		                (YEAR(DTBUHOTCHET), MONTH(kn.DTBUHOTCHET), kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt, knos.tdoc, knos.nomot)
	                )
                )

                Insert into @tree
                SELECT
                    [parentId] = CASE [treeLevel]
		            WHEN 0 THEN MAX(id) OVER (PARTITION BY [year], [month], KEYKRT, id_kart, typeDoc)
		            WHEN 1 THEN MAX(id) OVER (PARTITION BY [year], [month], KEYKRT, id_kart)
		            WHEN 3 THEN MAX(id) OVER (PARTITION BY [year], [month], KEYKRT)
		            WHEN 7 THEN MAX(id) OVER (PARTITION BY [year], [month])
		            WHEN 31 THEN MAX(id) OVER (PARTITION BY [year])
	            ELSE 0 END,
	            [id], [groupId], [rankInGr], [treeLevel],
	            [levelName] = CASE [treeLevel]
		            WHEN 0 THEN N'Документ'
		            WHEN 1 THEN N'Тип документа'
		            WHEN 3 THEN N'Карточка'
		            WHEN 7 THEN N'Перечень'
		            WHEN 31 THEN N'Месяц'
		            WHEN 63 THEN N'Год'
	            ELSE NULL END,
	            [searchkey] = CASE [treeLevel]
		            WHEN 0 THEN convert(nvarchar(25), [docum])
		            WHEN 1 THEN convert(nvarchar(25), [typeDoc])
		            WHEN 3 THEN convert(nvarchar(25), [id_kart])
		            WHEN 7 THEN convert(nvarchar(25), [keykrt])
		            WHEN 31 THEN convert(nvarchar(25), [month])
		            WHEN 63 THEN convert(nvarchar(25), [year])
	            ELSE NULL END,
	            [label] = CASE [treeLevel]
		            WHEN 0 THEN Convert(nvarchar(25), [docum])
		            WHEN 1 THEN Case [typeDoc] when 1 then N'Накладная' when 2 then N'Ведомость' when 3 then N'Акт' Else N'Карточка' End
		            WHEN 3 THEN [card]
		            WHEN 7 THEN CONVERT(NVARCHAR(10),[scroll])
		            WHEN 31 THEN DATENAME(MONTH,[period])
		            WHEN 63 THEN CONVERT(NVARCHAR(4),[year])
	            ELSE NULL END,
	            [count], [rootKey],
                [strKey] = '0x' + cast('' as xml).value('xs:hexBinary(sql:column(""rootKey"") )', 'varchar(max)')
                FROM grSubResult as gr
                WHERE  [treeLevel] IN ( {typeDoc} )
                ORDER BY [year] DESC, [month], KEYKRT DESC, id_kart desc, gr.[typeDoc] desc, [docum] desc;

            select * from @tree" + (rootKey == null ? ";" : $@" where [parentId] = (select id from @tree where [rootKey] = {Encoding.ASCII.GetString(rootKey)});");
            #endregion

            // map
            try
            {
                using (var ctx = new NomenclatureEntities())
                {
                    var dbName = ctx.Database.Connection.Database;

                    // list on nodes (flatted)
                    var result = ctx.Database.SqlQuery<TreeNode>(query).ToList();
                    var tree = result.FillRecursive();

                    Assert.IsNotNull(Encoding.ASCII.GetString(result[0].RootKey));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}