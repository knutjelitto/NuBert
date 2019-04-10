using Lingu.Grammars;

namespace Lingu.Earley
{
    public class EarleyStateFactory
    {
        public EarleyStateFactory(DottedRuleFactory dottedRules)
        {
            DottedRules = dottedRules;
        }

        private DottedRuleFactory DottedRules { get; }

        public EarleyState NewState(DottedRule dottedRule, int origin)
        {
            if (dottedRule.IsComplete)
            {
                return new CompletedState(dottedRule, origin);
            }

            if (dottedRule.PostDot is Nonterminal)
            {
                return new NonterminalState(dottedRule, origin);
            }

            return new TerminalState(dottedRule, origin);
        }

        public EarleyState NewState(Production production, int dot, int origin)
        {
            return NewState(DottedRules.Get(production, dot), origin);
        }

        public EarleyState NextState(EarleyState state)
        {
            if (state.DottedRule.IsComplete)
            {
                return null;
            }

            var dottedRule = state.DottedRule.Next;

            return NewState(dottedRule, state.Origin);
        }
    }
}