using System.Diagnostics;

namespace Lingu.Earley
{
    public class CompletedState : EarleyState
    {
        public CompletedState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
            Debug.Assert(dottedRule.Dot == dottedRule.Count);
        }

        public override bool AddTo(EarleySet set)
        {
            return set.Add(this);
        }
    }
}