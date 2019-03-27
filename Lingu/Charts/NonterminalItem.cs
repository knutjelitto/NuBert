using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        public override bool Add(EarleySet set)
        {
            return set.Add(this);
        }

        public bool IsSource(Nonterminal searchSymbol)
        {
            return !Dotted.IsComplete && Dotted.PostDot.Equals(searchSymbol);
        }

    }
}
