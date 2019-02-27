using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class Chart
    {
        public Chart()
        {
            this._earleySets = new List<EarleySet>();
        }

        public int Count => EarleySets.Count;

        public IReadOnlyList<EarleySet> EarleySets => this._earleySets;

        public bool Contains(int position, DottedRule dottedRule, int origin)
        {
            var earleySet = GetEarleySet(position);
            return earleySet.ContainsNormal(dottedRule, origin);
        }

        public bool Enqueue(int position, State state)
        {
            return GetEarleySet(position).Enqueue(state);
        }

        private EarleySet GetEarleySet(int position)
        {
            EarleySet earleySet;
            if (position < this._earleySets.Count)
            {
                earleySet = this._earleySets[position];
            }
            else
            {
                Debug.Assert(position == this._earleySets.Count);
                earleySet = new EarleySet();
                this._earleySets.Add(earleySet);
            }

            return earleySet;
        }

        private readonly List<EarleySet> _earleySets;
    }
}