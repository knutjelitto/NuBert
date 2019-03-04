﻿using System.Diagnostics;
using Pliant.Dotted;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class StateFactory
    {
        public StateFactory(DottedRuleRegistry dottedRuleRegistry)
        {
            DottedRuleRegistry = dottedRuleRegistry;
        }

        public State NewState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
        {
            return new NormalState(dottedRule, origin, parseNode);
        }

        public State NewState(Production production, int dot, int origin)
        {
            return NewState(DottedRuleRegistry.Get(production, dot), origin);
        }

        public State NextState(State state)
        {
            if (state.DottedRule.IsComplete)
            {
                return null;
            }

            var dottedRule = DottedRuleRegistry.GetNext(state.DottedRule);
            return NewState(dottedRule, state.Origin);
        }

        private DottedRuleRegistry DottedRuleRegistry { get; }
    }
}