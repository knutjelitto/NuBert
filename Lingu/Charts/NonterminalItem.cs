using System.Diagnostics;
using Lingu.Grammars;

namespace Lingu.Charts
{
    public class NonterminalItem : EarleyItem
    {
        public NonterminalItem(DottedRule dotted, int origin)
            : base(dotted, origin)
        {
            Debug.Assert(dotted.PostDot is Nonterminal);
        }

        public override bool AddTo(EarleySet set)
        {
            return set.Add(this);
        }

        public bool IsSource(Nonterminal nonterminal)
        {
            return !Dotted.IsComplete && Dotted.PostDot.Equals(nonterminal);
        }
    }
}