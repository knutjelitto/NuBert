using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicChart
    {
        private readonly List<DeterministicSet> _preComputedSets;

        public IReadOnlyList<DeterministicSet> Sets => this._preComputedSets;

        public DeterministicChart()
        {
            this._preComputedSets = new List<DeterministicSet>();
        }

        public bool Enqueue(int index, DeterministicState state)
        {
            DeterministicSet preComputedSet = null;
            if (this._preComputedSets.Count <= index)
            {
                preComputedSet = new DeterministicSet(index);
                this._preComputedSets.Add(preComputedSet);
            }
            else
            {
                preComputedSet = this._preComputedSets[index];
            }

            return preComputedSet.Enqueue(state);
        }

        public void Clear()
        {
            this._preComputedSets.Clear();
        }
    }
}
