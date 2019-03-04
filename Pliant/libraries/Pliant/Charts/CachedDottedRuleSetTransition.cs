﻿using Pliant.Dotted;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class CachedDottedRuleSetTransition
    {
        public CachedDottedRuleSetTransition(ISymbol symbol, DottedRuleSet dottedRuleSet, int origin)
        {
            Symbol = symbol;
            DottedRuleSet = dottedRuleSet;
            Origin = origin;
        }

        public DottedRuleSet DottedRuleSet { get; }
        public int Origin { get; }
        public ISymbol Symbol { get; }
    }
}