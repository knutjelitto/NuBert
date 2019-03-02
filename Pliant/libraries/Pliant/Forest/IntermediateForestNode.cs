using Pliant.Dotted;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class IntermediateForestNode : InternalForestNode, IIntermediateForestNode
    {
        public IntermediateForestNode(DottedRule dottedRule, int origin, int location)
            : base(origin, location)
        {
            DottedRule = dottedRule;
            this._hashCode = ComputeHashCode();
        }

        public DottedRule DottedRule { get; }

        public override ForestNodeType NodeType => ForestNodeType.Intermediate;

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is IIntermediateForestNode other && 
                   Location == other.Location && 
                   NodeType == other.NodeType && 
                   Origin == other.Origin && 
                   DottedRule.Equals(other.DottedRule);
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"({DottedRule}, {Origin}, {Location})";
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int) NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.GetHashCode());
        }

        private readonly int _hashCode;
    }
}