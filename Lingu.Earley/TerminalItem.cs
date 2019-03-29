using System.Diagnostics;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class TerminalItem : EarleyItem
    {
        public TerminalItem(DottedRule dotted, int origin)
            : base(dotted, origin)
        {
            Debug.Assert(dotted.PostDot is Terminal);
        }

        public override bool AddTo(EarleySet set)
        {
            return set.Add(this);
        }
    }
}