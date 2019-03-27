using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lingu.Charts
{
    public class CompletedItem : EarleyItem
    {
        public CompletedItem(DottedRule dotted, int origin)
            : base(dotted, origin)
        {
            Debug.Assert(dotted.Dot == dotted.Count);
        }

        public override bool Add(EarleySet set)
        {
            return set.Add(this);
        }
    }
}
