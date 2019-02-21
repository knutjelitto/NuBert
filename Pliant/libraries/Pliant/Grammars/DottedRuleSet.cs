using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Grammars
{
#if true
    public class DottedRuleSet : IEnumerable<DottedRule>
    {
        private readonly SortedSet<DottedRule> set = new SortedSet<DottedRule>(new DottedRuleComparer());

        private class DottedRuleComparer : IComparer<DottedRule>
        {
            public int Compare(DottedRule x, DottedRule y)
            {
                Debug.Assert(x != null, nameof(x) + " != null");
                Debug.Assert(y != null, nameof(y) + " != null");

                return x.Id.CompareTo(y.Id);
            }
        }

        public int Count => this.set.Count;

        public DottedRule[] ToArray()
        {
            return this.set.ToArray();
        }

        public bool Contains(DottedRule state)
        {
            return this.set.Contains(state);
        }

        public IEnumerator<DottedRule> GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is DottedRuleSet other && this.set.SequenceEqual(other.set);
        }

        public override int GetHashCode()
        {
            return HashCode.Compute(this.set);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Add(DottedRule state)
        {
            return this.set.Add(state);
        }

        public void Clear()
        {
            this.set.Clear();
        }
    }
#else
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
#endif
}
