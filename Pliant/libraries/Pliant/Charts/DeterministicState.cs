using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class DeterministicState
    {
        public DottedRuleAssortment DottedRuleSet { get; private set; }

        public int Origin { get; private set; }

        private readonly int _hashCode;
                
        public DeterministicState(DottedRuleAssortment dottedRuleSet, int origin)
        {
            DottedRuleSet = dottedRuleSet;
            Origin = origin;

            this._hashCode = ComputeHashCode(DottedRuleSet, Origin);  
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var deterministicState = obj as DeterministicState;
            if (deterministicState == null)
            {
                return false;
            }

            return deterministicState.Origin == Origin && DottedRuleSet.Equals(deterministicState.DottedRuleSet);
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
    }
}
