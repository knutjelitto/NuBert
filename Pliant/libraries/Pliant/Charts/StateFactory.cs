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

        private DottedRuleRegistry DottedRuleRegistry { get; }

        public EarleyItem NewState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
        {
            if (dottedRule.IsComplete)
            {
                return new CompletedState(dottedRule, origin, parseNode);
            }

            if (dottedRule.PostDotSymbol is NonTerminal)
            {
                return new PredictionState(dottedRule, origin, parseNode);
            }

            return new ScanState(dottedRule, origin, parseNode);
        }

        public EarleyItem NewState(Production production, int dot, int origin)
        {
            return NewState(DottedRuleRegistry.Get(production, dot), origin);
        }

        public EarleyItem NextState(EarleyItem state)
        {
            if (state.DottedRule.IsComplete)
            {
                return null;
            }

            var dottedRule = DottedRuleRegistry.GetNext(state.DottedRule);
            return NewState(dottedRule, state.Origin);
        }
    }
}