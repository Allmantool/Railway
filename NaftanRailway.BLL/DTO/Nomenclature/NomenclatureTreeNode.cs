using System;
using System.Collections.Generic;
using NaftanRailway.BLL.DTO.General;

namespace NaftanRailway.BLL.DTO.Nomenclature {
    public class NomenclatureTreeNode : TreeNodeBase<NomenclatureTreeNode>, ITreeNode<NomenclatureTreeNode> {
        public NomenclatureTreeNode(string name) : base(name) {
        }

        protected override NomenclatureTreeNode MySelf {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
