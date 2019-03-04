using System.Collections.Generic;
using System.Diagnostics;
using Pliant.Dotted;
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

        public EarleySet LastSet()
        {
            return this._earleySets[this._earleySets.Count - 1];
        }

        public bool ContainsNormal(int location, DottedRule dottedRule, int origin)
        {
            var earleySet = GetEarleySet(location);
            return earleySet.ContainsNormal(dottedRule, origin);
        }

        public bool Enqueue(int location, State state)
        {
            return state.Enqueue(GetEarleySet(location));
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