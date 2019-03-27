using System;
using System.Collections.Generic;
using System.Text;
using Lingu.Grammars;

namespace Lingu.Charts
{
    public class EarleyItemFactory
    {
        public EarleyItemFactory(DottedRuleFactory dottedRules)
        {
            DottedRules = dottedRules;
        }

        private DottedRuleFactory DottedRules { get; }

        public EarleyItem NewState(DottedRule dottedRule, int origin)
        {
            if (dottedRule.IsComplete)
            {
                return new CompletedItem(dottedRule, origin);
            }

            if (dottedRule.PostDot is Nonterminal)
            {
                return new NonterminalItem(dottedRule, origin);
            }

            return new TerminalItem(dottedRule, origin);
        }

        public EarleyItem NewState(Production production, int dot, int origin)
        {
            return NewState(DottedRules.Get(production, dot), origin);
        }

        public EarleyItem NextState(EarleyItem state)
        {
            if (state.Dotted.IsComplete)
            {
                return null;
            }

            var dottedRule = DottedRules.GetNext(state.Dotted);

            return NewState(dottedRule, state.Origin);
        }
    }
}
