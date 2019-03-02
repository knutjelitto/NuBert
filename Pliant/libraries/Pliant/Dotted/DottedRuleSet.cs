#if true
using System.Collections;
using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Dotted
{
    public class DottedRuleSet : IReadOnlyCollection<DottedRule>
    {
        public int Count => this.set.Count;

        public IEnumerator<DottedRule> GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(DottedRule state)
        {
            return this.set.Contains(state);
        }

        public override bool Equals(object obj)
        {
            return obj is DottedRuleSet other && this.set.SetEquals(other.set);
        }

        public override int GetHashCode()
        {
            return HashCode.Compute(this.set);
        }

        public bool Add(DottedRule state)
        {
            return this.set.Add(state);
        }

        public void Clear()
        {
            this.set.Clear();
        }

        private readonly HashSet<DottedRule> set = new HashSet<DottedRule>();
    }
}
#endif