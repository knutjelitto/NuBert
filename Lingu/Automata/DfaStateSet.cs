using System.Collections;
using System.Collections.Generic;
using Lingu.Tools;

namespace Lingu.Automata
{
    public class DfaStateSet : IEnumerable<DfaState>
    {
        private readonly HashSet<DfaState> set;

        public DfaStateSet()
            : this(new HashSet<DfaState>())
        {
        }

        public DfaStateSet(IEnumerable<DfaState> states)
            : this(new HashSet<DfaState>(states))
        {
        }

        private DfaStateSet(HashSet<DfaState> states)
        {
            this.set = states;
        }

        public bool Contains(DfaState state)
        {
            return this.set.Contains(state);
        }

        public DfaStateSet IntersectWith(DfaStateSet other)
        {
            var intersect = new HashSet<DfaState>(this.set);
            intersect.IntersectWith(other.set);

            return new DfaStateSet(intersect);
        }

        public void Add(DfaState state)
        {
            this.set.Add(state);
        }

        public bool IsEmpty => this.set.Count == 0;
        public int Count => this.set.Count;

        public DfaStateSet ExceptWith(DfaStateSet other)
        {
            var except = new HashSet<DfaState>(this.set);
            except.ExceptWith(other.set);

            return new DfaStateSet(except);
        }

        public IEnumerator<DfaState> GetEnumerator()
        {
            return this.set.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            return obj is DfaStateSet other && this.set.SetEquals(other.set);
        }

        public override int GetHashCode()
        {
            return this.set.SequenceHash();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static bool operator ==(DfaStateSet s1, DfaStateSet s2)
        {
            if (s1 is null || s2 is null)
            {
                return false;
            }

            return s1.Equals(s2);
        }

        public static bool operator !=(DfaStateSet s1, DfaStateSet s2)
        {
            return !(s1 == s2);
        }
    }
}
