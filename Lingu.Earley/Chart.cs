using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lingu.Earley
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

        public EarleySet this[int index]
        {
            get
            {
                EarleySet earleySet;
                if (index < this.sets.Count)
                {
                    earleySet = this.sets[index];
                }
                else
                {
                    Debug.Assert(index == this.sets.Count);
                    earleySet = new EarleySet();
                    this.sets.Add(earleySet);
                }

                return earleySet;
            }
        }

        public EarleySet Current => this.sets[this.sets.Count - 1];

        public bool Contains(int location, DottedRule dottedRule, int origin)
        {
            return this[location].Contains(dottedRule, origin);
        }

        public bool Add(int location, EarleyState item)
        {
            return item.AddTo(this[location]);
        }

        private readonly List<EarleySet> sets;
    }
}
