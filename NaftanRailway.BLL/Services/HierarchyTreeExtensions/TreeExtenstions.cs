using NaftanRailway.BLL.DTO.Nomenclature;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NaftanRailway.BLL.Services.HierarchyTreeExtensions {
    /// <summary>
    /// It's the class with specific method for working with hierarchy structure through LINQ
    /// </summary>
    public static class TreeExtenstions {
        /// <summary>
        /// It fills tree recursive (up to down) from ado.net
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static List<TreeNode> FillRecursive(this IList<DataRow> rows, int parentId = 0) {
            //top nodes
            var roots = rows.Where(x => Convert.ToInt32(x["parentId"]) == parentId);

            var result = roots.Select(item => new TreeNode(item["levelName"].ToString()) {
                Id = Convert.ToInt32(item["Id"]),
                Children = FillRecursive(rows, Convert.ToInt32(item["Id"])),
                Label = item["label"].ToString(),
                SearchKey = item["searchkey"].ToString(),
                Count = Convert.ToInt32(item["count"]),
            }).ToList();

            return result;
        }

        /// <summary>
        /// It fills tree recursive (up to down) from node collection
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static List<TreeNode> FillRecursive(this IList<TreeNode> rows, long parentId = 0) {
            //top nodes
            var roots = rows.Where(x => x.ParentId == parentId).ToList();

            //build hierarchy
            var result = roots.Select(item => new TreeNode(item.LevelName) {
                Id = item.Id,
                Children = FillRecursive(rows, item.Id).Count() < 1 ? new List<TreeNode>() { new TreeNode() } : FillRecursive(rows, item.Id),
                Label = item.Label,
                SearchKey = item.SearchKey,
                Count = item.Count,
                GroupId = item.GroupId,
                ParentId = item.ParentId,
                RankInGr = item.RankInGr,
                TreeLevel = item.TreeLevel
            }).ToList();

            return result;
        }

        /// <summary>
        /// Work throught stack
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNode> Descendants(this TreeNode root) {
            var nodes = new Stack<TreeNode>(new[] { root });

            while (nodes.Any()) {
                TreeNode node = nodes.Pop();

                yield return node;

                foreach (var n in node.Children) nodes.Push(n);
            }
        }
    }
}
