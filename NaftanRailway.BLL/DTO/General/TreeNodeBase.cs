using System.Collections.Generic;
using System.Linq;

namespace NaftanRailway.BLL.DTO.General {
    /// <summary>
    /// it represents a generic abstract(base) class for hierarchy tree node implementions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeNodeBase<T> : ITreeNode<T> where T : class, ITreeNode<T> {
        /// <summary>
        /// It returns context ('this'). ('this' couldn't straight cast to type T).
        /// Implemention in conrete class must override this property and to return
        /// ( get { return this; } )
        /// </summary>
        protected abstract T MySelf { get; }

        public TreeNodeBase(string name) {
            Name = name;
            Children = new List<T>();
        }

        public int Id {
            get; set;
        }
        public string Name {
            get; private set;
        }
        /// <summary>
        /// the parent has a public setter due to the interface
        /// </summary>
        public T Parent {
            get; set;
        }
        public IList<T> Children {
            get;  set;
        }

        public bool IsLeaf {
            get {
                return Children.Count == 0;
            }
        }

        public bool IsRoot {
            get {
                return Parent == null;
            }
        }

        public void AddChild(T child) {
            child.Parent = MySelf;
            Children.Add(child);
        }
        public void AddChildren(IEnumerable<T> children) {
            foreach (T child in children)
                AddChild(child);
        }

        /// <summary>
        /// List of Leaf Nodes
        /// </summary>
        /// <returns></returns>
        public List<T> GetLeafNodes() {
            return Children.Where(x => x.IsLeaf).ToList();
        }
        public List<T> GetNonLeafNodes() {
            return Children.Where(x => !x.IsLeaf).ToList();
        }

        /// <summary>
        /// Get the Root Node of the Tree
        /// </summary>
        /// <returns></returns>
        public T GetRootNode() {
            if (Parent == null)
                return MySelf;

            return Parent.GetRootNode();
        }

        /// <summary>
        /// Dot separated name from the Root to this Tree Node
        /// </summary>
        /// <returns></returns>
        public string GetFullyQualifiedName() {
            if (Parent == null) return Name;

            return string.Format("{0}.{1}", Parent.GetFullyQualifiedName(), Name);
        }
    }
}
