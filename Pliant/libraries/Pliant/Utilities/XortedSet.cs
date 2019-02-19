using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.Utilities
{
#if false
    public class DottedSet : HashSet<DottedRule>
    {
    }
#else
    public class DottedSet : SortedSet<DottedRule>
    {
        public DottedSet() : base(new DottedRuleComparer())
        {
        }

        private class DottedRuleComparer : IComparer<DottedRule>
        {
            public int Compare(DottedRule x, DottedRule y)
            {
                Debug.Assert(x != null);
                Debug.Assert(y != null);
                return x.GetHashCode().CompareTo(y.GetHashCode());
            }
        }
    }
#endif
}