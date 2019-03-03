﻿using Pliant.Dotted;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public sealed class DeterministicState : ValueEqualityBase<DeterministicState>
    {
        public DeterministicState(DottedRuleSet dottedRuleSet, int origin)
            : base((dottedRuleSet, origin))
        {
            DottedRuleSet = dottedRuleSet;
            Origin = origin;
        }

        public DottedRuleSet DottedRuleSet { get; }
        public int Origin { get; }

        public override bool ThisEquals(DeterministicState other)
        {
            return Origin.Equals(other.Origin) &&
                   DottedRuleSet.Equals(other.DottedRuleSet);
        }
    }
}