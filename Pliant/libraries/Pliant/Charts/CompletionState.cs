using Pliant.Dotted;
using Pliant.Forest;

namespace Pliant.Charts
{
    public class CompletionState : StateBase
    {
        public CompletionState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
            : base(dottedRule, origin, parseNode)
        {
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }
    }
}