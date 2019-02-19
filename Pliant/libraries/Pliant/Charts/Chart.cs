using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class Chart //: IChart
    {
        private readonly List<EarleySet> _earleySets;
        
        public IReadOnlyList<IEarleySet> EarleySets => this._earleySets;

        public Chart()
        {
            this._earleySets = new List<EarleySet>();
        }

        public bool Enqueue(int position, State state)
        {
            IEarleySet earleySet = GetEarleySet(position);
            return earleySet.Enqueue(state);
        }

        public int Count => EarleySets.Count;

        public bool Contains(int position, IDottedRule dottedRule, int origin)
        {
            var earleySet = GetEarleySet(position);
            return earleySet.ContainsNormal(dottedRule, origin);
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
                earleySet = new EarleySet(position);
                this._earleySets.Add(earleySet);
            }

            return earleySet;
        }
    }
}