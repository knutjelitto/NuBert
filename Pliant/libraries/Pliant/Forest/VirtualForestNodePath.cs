using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class VirtualForestNodePath : ValueEqualityBase<VirtualForestNodePath>
    {
        public VirtualForestNodePath(TransitionState transitionState, IForestNode forestNode)
        {
            TransitionState = transitionState;
            ForestNode = forestNode;
        }

        public TransitionState TransitionState { get; }
        public IForestNode ForestNode { get; }

        protected override bool ThisEquals(VirtualForestNodePath other)
        {
            return TransitionState.Equals(other.TransitionState) &&
                   ForestNode.Equals(other.ForestNode);
        }

        protected override object ThisHashCode => (TransitionState, ForestNode);
    }
}