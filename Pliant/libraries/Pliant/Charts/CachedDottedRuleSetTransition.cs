using Pliant.Grammars;

namespace Pliant.Charts
{
    public class CachedDottedRuleSetTransition
    {
        public CachedDottedRuleSetTransition(Symbol symbol, DottedRuleAssortment dottedRuleSet, int origin)
        {
            Symbol = symbol;
            DottedRuleSet = dottedRuleSet;
            Origin = origin;
        }

        public DottedRuleAssortment DottedRuleSet { get; }
        public int Origin { get; }
        public Symbol Symbol { get; }
    }
}