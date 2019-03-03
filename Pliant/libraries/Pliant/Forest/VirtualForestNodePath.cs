using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class VirtualForestNodePath : ValueEqualityBase<VirtualForestNodePath>
    {
        public VirtualForestNodePath(TransitionState transitionState, IForestNode forestNode)
            : base((transitionState, forestNode))
        {
            TransitionState = transitionState;
            ForestNode = forestNode;
        }

        public TransitionState TransitionState { get; }
        public IForestNode ForestNode { get; }

        public override bool ThisEquals(VirtualForestNodePath other)
        {
            return TransitionState.Equals(other.TransitionState) &&
                   ForestNode.Equals(other.ForestNode);
        }
    }
}