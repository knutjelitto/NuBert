using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class DottedRuleSet : IEnumerable<DottedRule>
    {
        private readonly HashSet<DottedRule> set = new HashSet<DottedRule>();

        public int Count => this.set.Count;

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
            return obj is DottedRuleSet other && this.set.SetEquals(other.set);
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
}
