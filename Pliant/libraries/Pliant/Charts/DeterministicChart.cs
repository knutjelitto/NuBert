using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicChart
    {
        public DeterministicChart()
        {
            this.preComputedSets = new List<DeterministicSet>();
        }

        public IReadOnlyList<DeterministicSet> Sets => this.preComputedSets;

        public void Clear()
        {
            this.preComputedSets.Clear();
        }

        public bool Enqueue(int index, DeterministicState state)
        {
            DeterministicSet preComputedSet;
            if (this.preComputedSets.Count <= index)
            {
                preComputedSet = new DeterministicSet(index);
                this.preComputedSets.Add(preComputedSet);
            }
            else
            {
                preComputedSet = this.preComputedSets[index];
            }

            return preComputedSet.Enqueue(state);
        }

        private readonly List<DeterministicSet> preComputedSets;
    }
}