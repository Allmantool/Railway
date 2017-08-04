using System.Collections.Generic;

namespace NaftanRailway.BLL.DTO.General {
    /// <summary>
    /// The generic interface that represents node in a hierarchy tree 
    /// (composite pattern)
    /// </summary>
    public interface ITreeNode<T> {
        int Id { get; set; }
        string Name { get; }
        T Parent { get; set; }
        IList<T> Children { get;  }

        /// <summary>
        /// True if a Leaf Node
        /// </summary>
        bool IsLeaf {
            get;
        }

        /// <summary>
        /// True if the Root of the Tree
        /// </summary>
        bool IsRoot {
            get;
        }

        T GetRootNode();
        string GetFullyQualifiedName();

        void AddChild(T child);
        void AddChildren(IEnumerable<T> children);
    }
}
