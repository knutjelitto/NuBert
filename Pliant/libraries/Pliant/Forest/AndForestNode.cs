using System.Collections.Generic;
using System.Diagnostics;

namespace Pliant.Forest
{
    public class AndForestNode
    {
        public AndForestNode(IForestNode first, IForestNode second)
        {
            Debug.Assert(first != null && second != null);
            this.children = new List<IForestNode> {first, second};
        }

        public AndForestNode(IForestNode first)
        {
            Debug.Assert(first != null);
            this.children = new List<IForestNode> { first };
        }

        public static AndForestNode Make(IForestNode first, IForestNode second)
        {
            if (second == null)
            {
                return new AndForestNode(first);
            }

            return new AndForestNode(first, second);
        }

        public IForestNode First => this.children[0];
        public IForestNode Second => this.children[1];

        public AndForestNode Clone()
        {
            if (this.children.Count == 1)
            {
                return new AndForestNode(this.children[0]);
            }

            return new AndForestNode(this.children[0], this.children[1]);
        }

        public IReadOnlyList<IForestNode> Children => this.children;

        private readonly List<IForestNode> children;
    }
}