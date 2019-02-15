using System.Collections.Generic;

namespace Pliant.Forest
{
    public class AndForestNode : IAndForestNode
    {
        public IReadOnlyList<IForestNode> Children => this._children;

        private readonly List<IForestNode> _children;

        public AndForestNode()
        {
            this._children = new List<IForestNode>();
        }

        public void AddChild(IForestNode orNode)
        {
            this._children.Add(orNode);
        }
    }
}