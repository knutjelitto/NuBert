using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class DeterministicState
    {
        public DeterministicState(DottedRuleAssortment dottedRuleSet, int origin)
        {
            DottedRuleSet = dottedRuleSet;
            Origin = origin;

            this.hashCode = HashCode.Compute(DottedRuleSet.GetHashCode(), Origin.GetHashCode());
        }

        public DottedRuleAssortment DottedRuleSet { get; }

        public int Origin { get; }

        public override bool Equals(object obj)
        {
            return obj is DeterministicState other &&
                   Origin.Equals(other.Origin) &&
                   DottedRuleSet.Equals(other.DottedRuleSet);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        private readonly int hashCode;
    }
}