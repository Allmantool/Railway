using NaftanRailway.BLL.DTO.General;

namespace NaftanRailway.BLL.DTO.Nomenclature {
    public class TreeNode : TreeNodeBase<TreeNode> {
        public TreeNode(string name) : base(name) {
            //Debug.Write(name);
        }

        protected override TreeNode MySelf => this;

        public int ParentId { get; set; }
        public int ElementId { get; set; }
        public string LevelName { get; set; }
        public string Label { get; set; }
        public string Searchkey { get; set; }
        /// <summary>
        /// Charges count
        /// </summary>
        public int Count { get; set; }
    }
}
