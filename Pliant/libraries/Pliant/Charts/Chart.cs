using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class Chart : IChart
    {
        private readonly List<EarleySet> _earleySets;
        
        public IReadOnlyList<IEarleySet> EarleySets => this._earleySets;

        public Chart()
        {
            this._earleySets = new List<EarleySet>();
        }

        public bool Enqueue(int index, IState state)
        {
            IEarleySet earleySet = GetEarleySet(index);
            return earleySet.Enqueue(state);
        }

        public int Count => EarleySets.Count;

        public bool Contains(int index, StateType stateType, IDottedRule dottedRule, int origin)
        {
            var earleySet = GetEarleySet(index);
            return earleySet.Contains(stateType, dottedRule, origin);
        }

        private EarleySet GetEarleySet(int index)
        {
            EarleySet earleySet = null;
            if (this._earleySets.Count <= index)
            {
                earleySet = new EarleySet(index);
                this._earleySets.Add(earleySet);
            }
            else
            {
                earleySet = this._earleySets[index];
            }

            return earleySet;
        }
    }
}