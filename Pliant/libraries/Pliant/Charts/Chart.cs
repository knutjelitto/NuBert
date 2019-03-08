using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pliant.Charts
{
    public class Chart : IReadOnlyList<EarleySet>
    {
        public Chart()
        {
            this.sets = new List<EarleySet>();
        }

        public int Count => this.sets.Count;

        public IEnumerator<EarleySet> GetEnumerator()
        {
            return this.sets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public EarleySet this[int index] => this.sets[index];

        public EarleySet Current => this.sets[this.sets.Count - 1];

        public bool Contains(int location, DottedRule dottedRule, int origin)
        {
            return GetEarleySet(location).Contains(dottedRule, origin);
        }

        public bool Enqueue(int location, EarleyItem state)
        {
            return state.Enqueue(GetEarleySet(location));
        }

        private EarleySet GetEarleySet(int location)
        {
            EarleySet earleySet;
            if (location < this.sets.Count)
            {
                earleySet = this.sets[location];
            }
            else
            {
                Debug.Assert(location == this.sets.Count);
                earleySet = new EarleySet();
                this.sets.Add(earleySet);
            }

            return earleySet;
        }

        private readonly List<EarleySet> sets;
    }
}