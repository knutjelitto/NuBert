using Pliant.Utilities;

namespace Pliant.Forest
{
    public abstract class ForestNodeBase : IForestNode
    {
        protected ForestNodeBase(int origin, int location)
        {
            Origin = origin;
            Location = location;
            this._hashCode = ComputeHashCode();
        }

        public int Location { get; }

        public abstract ForestNodeType NodeType { get; }

        public int Origin { get; }

        public abstract void Accept(IForestNodeVisitor visitor);

        public override bool Equals(object obj)
        {
            return obj is ForestNodeBase nodeBase && 
                   Location == nodeBase.Location && 
                   NodeType == nodeBase.NodeType && 
                   Origin == nodeBase.Origin;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int) NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode());
        }

        private readonly int _hashCode;
    }
}