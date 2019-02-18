using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class VirtualForestNodePath
    {
        private readonly int _hashCode;
        public TransitionState TransitionState { get; private set; }
        public IForestNode ForestNode { get; private set; }

        public VirtualForestNodePath(TransitionState transitionState, IForestNode forestNode)
        {
            TransitionState = transitionState;
            ForestNode = forestNode;
            this._hashCode = ComputeHashCode(TransitionState, ForestNode);
        }

        private static int ComputeHashCode(TransitionState transitionState, IForestNode forestNode)
        {
            return HashCode.Compute(
                transitionState.GetHashCode(), 
                forestNode.GetHashCode());
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var virtualForestNodePath = obj as VirtualForestNodePath;
            if (virtualForestNodePath == null)
            {
                return false;
            }

            return TransitionState.Equals(virtualForestNodePath.TransitionState)
                && ForestNode.Equals(virtualForestNodePath.ForestNode);
        }
    }
}