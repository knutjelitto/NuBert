using Pliant.Forest;

namespace Pliant.Charts
{
    public abstract class RuleState : State
    {
        protected RuleState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
            : base(dottedRule, origin, parseNode)
        {
            this.hashCode = (DottedRule, Origin).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RuleState other &&
                   DottedRule.Equals(other.DottedRule) &&
                   Origin.Equals(other.Origin);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return $"{DottedRule}\t\t({Origin})";
        }

        private readonly int hashCode;
    }
}