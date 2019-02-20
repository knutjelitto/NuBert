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

            this._hashCode = ComputeHashCode(DottedRuleSet, Origin);
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
            return this._hashCode;
        }

        private static int ComputeHashCode(DottedRuleAssortment dottedRuleSet)
        {
            return dottedRuleSet.GetHashCode();
        }

        private static int ComputeHashCode(DottedRuleAssortment dottedRuleSet, int origin)
        {
            return HashCode.Compute(dottedRuleSet.GetHashCode(), origin.GetHashCode());
        }

        private readonly int _hashCode;
    }
}