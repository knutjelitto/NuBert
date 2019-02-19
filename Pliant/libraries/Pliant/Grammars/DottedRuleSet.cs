using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Pliant.Grammars
{
    public class DottedRuleSet : SortedSet<DottedRule>
    {
        public DottedRuleSet() : base(new DottedRuleComparer())
        {
        }

        private class DottedRuleComparer : IComparer<DottedRule>
        {
            public int Compare(DottedRule x, DottedRule y)
            {
                Debug.Assert(x != null, nameof(x) + " != null");
                Debug.Assert(y != null, nameof(y) + " != null");

                return x.Id.CompareTo(y.Id);
            }
        }
    }
}
