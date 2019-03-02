﻿using Pliant.Dotted;
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

        public State NewState(Production production, int location, int origin)
        {
            var dottedRule = DottedRuleRegistry.Get(production, location);
            return NewState(dottedRule, origin);
        }

        public State NewState(DottedRule dottedRule, int origin, IForestNode forestNode = null)
        {
            return forestNode == null
                       ? new NormalState(dottedRule, origin)
                       : new NormalState(dottedRule, origin, forestNode);
        }

        public State NextState(State state, IForestNode parseNode = null)
        {
            if (state.DottedRule.IsComplete)
            {
                return null;
            }

            var dottedRule = DottedRuleRegistry.GetNext(state.DottedRule);
            return parseNode == null
                       ? new NormalState(dottedRule, state.Origin)
                       : new NormalState(dottedRule, state.Origin, parseNode);
        }

        private DottedRuleRegistry DottedRuleRegistry { get; }
    }
}