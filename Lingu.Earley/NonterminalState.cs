using System.Diagnostics;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class NonterminalState : EarleyState
    {
        public NonterminalState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
            Debug.Assert(dottedRule.PostDot is Nonterminal);
        }

        public override bool AddTo(EarleySet set)
        {
            return set.Add(this);
        }

        public bool IsSource(Nonterminal nonterminal)
        {
            return !DottedRule.IsComplete && DottedRule.PostDot.Equals(nonterminal);
        }
    }
}