using System.Collections.Generic;

namespace Pliant.Forest
{
    public class AndForestNode : IAndForestNode
    {
        public AndForestNode()
        {
            this._children = new List<IForestNode>();
        }

        public IReadOnlyList<IForestNode> Children => this._children;

        public void AddChild(IForestNode orNode)
        {
            this._children.Add(orNode);
        }

        private readonly List<IForestNode> _children;
    }
}