using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class PredictionState : EarleyItem
    {
        public PredictionState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : base(dottedRule, origin, parseNode)
        {
        }

        public bool IsSource(Symbol searchSymbol)
        {
            return !DottedRule.IsComplete && DottedRule.PostDotSymbol.Is(searchSymbol);
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }
    }
}