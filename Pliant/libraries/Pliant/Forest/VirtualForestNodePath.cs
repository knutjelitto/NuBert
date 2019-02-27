using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class VirtualForestNodePath
    {
        public VirtualForestNodePath(TransitionState transitionState, IForestNode forestNode)
        {
            TransitionState = transitionState;
            ForestNode = forestNode;
        }

        public TransitionState TransitionState { get; }
        public IForestNode ForestNode { get; }

        public override int GetHashCode()
        {
            return HashCode.Compute(TransitionState.GetHashCode(), ForestNode.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj is VirtualForestNodePath other &&
                   TransitionState.Equals(other.TransitionState) &&
                   ForestNode.Equals(other.ForestNode);
        }
    }
}