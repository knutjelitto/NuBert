using System;
using System.Collections.Generic;
using Pliant.Forest;

namespace Pliant.Tests.Common.Forest
{
    public abstract class FakeInternalForestNode : IInternalForestNode
    {
        protected FakeInternalForestNode(int origin, int location, params IAndForestNode[] children)
        {
            Origin = origin;
            Location = location;
            this._children = new List<IAndForestNode>(children);
        }

        public IReadOnlyList<IAndForestNode> Children => this._children;

        public int Location { get; }

        public abstract ForestNodeType NodeType { get; }

        public int Origin { get; }

        public void Accept(IForestNodeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public void AddUniqueFamily(IForestNode trigger)
        {
            throw new NotImplementedException();
        }

        public void AddUniqueFamily(IForestNode trigger, IForestNode source)
        {
            throw new NotImplementedException();
        }

        private readonly List<IAndForestNode> _children;
    }
}