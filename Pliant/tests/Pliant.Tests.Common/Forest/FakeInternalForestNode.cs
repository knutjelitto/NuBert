using System.Collections.Generic;
using Pliant.Forest;

namespace Pliant.Tests.Common.Forest
{
    public abstract class FakeInternalForestNode : InternalForestNode
    {
        protected FakeInternalForestNode(int origin, int location, params IAndForestNode[] children)
            : base(origin, location)
        {
            this.children = new List<IAndForestNode>(children);
        }

        public override IReadOnlyList<IAndForestNode> Children => this.children;

        private readonly List<IAndForestNode> children;
    }
}