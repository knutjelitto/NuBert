using System.Collections.Generic;

namespace Pliant.Forest
{
    public class AndForestNode : IAndForestNode
    {
        public AndForestNode(params IForestNode[] nodes)
        {
            this.children = new List<IForestNode>(nodes);
        }

        public IReadOnlyList<IForestNode> Children => this.children;

        public void AddChild(IForestNode orNode)
        {
            this.children.Add(orNode);
        }

        private readonly List<IForestNode> children;
    }
}