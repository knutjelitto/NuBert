using Pliant.Charts;

namespace Pliant.Forest
{
    public sealed class IntermediateForestNode : InternalForestNode, IIntermediateForestNode
    {
        public IntermediateForestNode(DottedRule dottedRule, int origin, int location)
            : base(origin, location)
        {
            DottedRule = dottedRule;
            this.hashCode = (DottedRule, Origin, Location).GetHashCode();
        }

        public DottedRule DottedRule { get; }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is IIntermediateForestNode other &&
                   DottedRule.Equals(other.DottedRule) &&
                   Origin == other.Origin &&
                   Location == other.Location;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return $"({DottedRule}, {Origin}, {Location})";
        }

        private readonly int hashCode;
    }
}