using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.Utilities
{
    public class XortedSet<T> : SortedSet<T>
    {
    }

    public class DottedSet : SortedSet<DottedRule>
    {
        public DottedSet() : base(new DottedRuleComparer())
        {
        }

        #region  not sortable (modify ReSharper template to catch these cases)

        private class DottedRuleComparer : IComparer<DottedRule>
        {
            public int Compare(DottedRule x, DottedRule y)
            {
                Debug.Assert(x != null);
                Debug.Assert(y != null);
                return x.GetHashCode().CompareTo(y.GetHashCode());
            }
        }

        #endregion
    }
}