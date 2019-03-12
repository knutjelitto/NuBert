using Pliant.Forest;

namespace Pliant.Charts
{
    public class ScanState : EarleyItem
    {
        public ScanState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : base(dottedRule, origin, parseNode)
        {
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }
    }
}