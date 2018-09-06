namespace NaftanRailway.UnitTests.General
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// It's the class with specific method for working with hierarchy structure through LINQ
    /// </summary>
    public static class TreeExtenstions
    {
        public static List<TreeNode> FillRecursive(this IList<DataRow> rows, int parentId = 0)
        {
            // top nodes
            var roots = rows.Where(x => Convert.ToInt32((object) x["parentId"]) == parentId);

            var result = roots.Select(item => new TreeNode(item["levelName"].ToString())
            {
                Id = Convert.ToInt32(item["id"]),
                Children = FillRecursive(rows, Convert.ToInt32(item["id"])),
                Label = item["label"].ToString(),
                SearchKey = item["searchkey"].ToString(),
                Count = Convert.ToInt32(item["count"]),
            }).ToList();

            return result;
        }

        public static IEnumerable<TreeNode> Descendants(this TreeNode root)
        {
            var nodes = new Stack<TreeNode>(new[] { root });

            while (nodes.Any())
            {
                TreeNode node = nodes.Pop();

                yield return node;

                foreach (var n in node.Children)
                {
                    nodes.Push(n);
                }
            }
        }

        /// <summary>
        /// It fills tree recursive (up to down) from node collection
        /// </summary>
        /// <returns> Tree Node.</returns>
        public static List<TreeNode> FillRecursive(this IList<TreeNode> rows, long parentId = 0)
        {
            // top nodes
            var roots = rows.Where(x => x.ParentId == parentId).ToList();

            // build hierarchy
            var result = roots.Select(item => new TreeNode(item.LevelName)
            {
                Id = item.Id,
                Children = FillRecursive(rows, item.Id),
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
    }
}