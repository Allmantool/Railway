using NaftanRailway.BLL.DTO.General;
using System.ComponentModel.DataAnnotations;

namespace NaftanRailway.BLL.DTO.Nomenclature {
    public class TreeNode : TreeNodeBase<TreeNode> {
        public TreeNode() { }

        public TreeNode(string name) : base(name) {
            //Debug.Write(name);
        }

        protected override TreeNode MySelf => this;

        [Key()/*, Column("id")*/]
        public override long Id { get; set; }
        public long ParentId { get; set; }
        public long GroupId { get; set; }
        public long RankInGr { get; set; }
        public int TreeLevel { get; set; }
        public string Label { get; set; }
        public string SearchKey { get; set; }
        public int Count { get; set; }
    }
}