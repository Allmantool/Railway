using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaftanRailway.BLL.DTO.General;

namespace NaftanRailway.UnitTests.General {
    [TestClass]
    public class HierarchyTree {
        [TestMethod]
        public void SetUp() {

            //            --The order must be same in each aggregation functions
            //; WITH grSubResult AS(
            //    SELECT[grNumber] = DENSE_RANK() OVER(ORDER BY kn.KEYKRT DESC),

            //           [rankInGr] = RANK() OVER(partition by kn.KEYKRT ORDER BY kn.KEYKRT DESC, knos.id_kart desc),

            //           [treeLevel] = GROUPING_ID(kn.KEYKRT, kn.NKRT, knos.id_kart),
            //           --[level_card] = GROUPING(knos.id_kart),

            //           [scroll] = kn.NKRT, kn.KEYKRT,

            //           [card] = knos.NKRT, knos.id_kart,

            //           [count] = COUNT(*)

            //    FROM krt_Naftan_orc_sapod AS knos INNER JOIN krt_Naftan AS kn

            //        ON kn.KEYKRT = knos.keykrt

            //    GROUP BY GROUPING SETS(
            //        (kn.KEYKRT, kn.NKRT),
            //        (kn.KEYKRT, kn.NKRT, knos.id_kart, knos.nkrt)
            //	)
            //)

            //SELECT*
            //FROM grSubResult
            //WHERE[grNumber] <= 1
            //ORDER BY KEYKRT DESC, id_kart desc;

            //Assert
            //var nodesHeap = new[] {
            //    new TreeNode() {Id = 1 },
            //    new TreeNode() {Id = 2, ParentId = 1 },
            //    new TreeNode() {Id = 3, ParentId = 1 },
            //    new TreeNode() {Id = 4, ParentId = 3 },
            //    new TreeNode() {Id = 5 },
            //};
            //Act


            //Arrange
        }
    }
}
