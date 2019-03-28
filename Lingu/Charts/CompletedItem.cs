using System.Diagnostics;

namespace Lingu.Charts
{
    public class CompletedItem : EarleyItem
    {
        public CompletedItem(DottedRule dotted, int origin)
            : base(dotted, origin)
        {
            Debug.Assert(dotted.Dot == dotted.Count);
        }

        public override bool AddTo(EarleySet set)
        {
            return set.Add(this);
        }
    }
}