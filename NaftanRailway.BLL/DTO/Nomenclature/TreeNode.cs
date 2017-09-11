using NaftanRailway.BLL.DTO.General;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public int GroupId { get; set; }
        public int RankInGr { get; set; }
        public short TreeLevel { get; set; }
        public string Label { get; set; }
        public string SearchKey { get; set; }
        public long Count { get; set; }
        public byte[] RootKey { get; set; }
        /// <summary>
        /// Return byte array instead of base64String()
        /// </summary>
        //public int[] ByteArray { get { return RootKey.Select(b => (int)b).ToArray(); } }
    }
}