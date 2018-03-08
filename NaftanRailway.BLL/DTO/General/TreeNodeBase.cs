using System.Collections.Generic;
using System.Linq;

namespace NaftanRailway.BLL.DTO.General {
    /// <summary>
    /// it represents a generic abstract(base) class for hierarchy tree node implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TreeNodeBase<T> : ITreeNode<T> where T : class, ITreeNode<T> {
        /// <summary>
        /// It returns context ('this'). ('this' couldn't straight cast to type T).
        /// Implementation in concrete class must override this property and to return
        /// ( get { return this; } )
        /// </summary>
        protected abstract T MySelf { get; }

        private readonly IList<T> _children = new List<T>();

        /// <summary>
        /// Requered for EF SqlQuery mapping
        /// </summary>
        public TreeNodeBase() {
        }

        public TreeNodeBase(string levelName) {
            this.LevelName = levelName;
            this.Children = new List<T>();
        }

        public virtual long Id {
            get; set;
        }
        /// <summary>
        /// It's a custom description of level tree
        /// </summary>
        public string LevelName {
            get; private set;
        }
        /// <summary>
        /// the parent has a public setter due to the interface
        /// </summary>
        public T Parent {
            get; set;
        }
        public IList<T> Children {
            get { return this._children; }
            set { this.AddChildren(value); }
        }

        public bool IsLeaf => this.Children.Count == 0;

        public bool IsRoot => this.Parent == null;

        public void AddChild(T child) {
            child.Parent = this.MySelf;
            this._children.Add(child);
        }
        public void AddChildren(IEnumerable<T> chNodes) {
            foreach (T child in chNodes)
                this.AddChild(child);
        }

        /// <summary>
        /// List of Leaf Nodes
        /// </summary>
        /// <returns></returns>
        public List<T> GetLeafNodes() {
            return this.Children.Where(x => x.IsLeaf).ToList();
        }
        public List<T> GetNonLeafNodes() {
            return this.Children.Where(x => !x.IsLeaf).ToList();
        }

        /// <summary>
        /// Get the Root Node of the Tree
        /// </summary>
        /// <returns></returns>
        public T GetRootNode() {
            if (this.Parent == null)
                return this.MySelf;

            return this.Parent.GetRootNode();
        }

        /// <summary>
        /// Dot separated name from the Root to this Tree Node
        /// </summary>
        /// <returns></returns>
        public string GetFullyQualifiedName() {
            if (this.Parent == null) return this.LevelName;

            return $"{this.Parent.GetFullyQualifiedName()}.{ this.LevelName}";
        }
    }
}
