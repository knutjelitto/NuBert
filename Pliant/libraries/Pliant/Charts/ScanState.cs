using Pliant.Dotted;
using Pliant.Forest;

namespace Pliant.Charts
{
    public class ScanState : RuleState
    {
        public ScanState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
            : base(dottedRule, origin, parseNode)
        {
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }
    }
}