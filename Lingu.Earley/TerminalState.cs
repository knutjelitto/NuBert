using System.Diagnostics;
using Lingu.Grammars;

namespace Lingu.Earley
{
    public class TerminalState : EarleyState
    {
        public TerminalState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
            Debug.Assert(dottedRule.PostDot is Terminal);
        }

        public override bool AddTo(EarleySet set)
        {
            return set.Add(this);
        }
    }
}