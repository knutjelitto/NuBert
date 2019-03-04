using System.Collections.Generic;

namespace Pliant.Charts
{
    public class DeterministicChart
    {
        public DeterministicChart()
        {
            this.sets = new List<DeterministicSet>();
        }

        public IReadOnlyList<DeterministicSet> Sets => this.sets;

        public bool Enqueue(int index, DeterministicState state)
        {
            DeterministicSet set;
            if (this.sets.Count <= index)
            {
                set = new DeterministicSet(index);
                this.sets.Add(set);
            }
            else
            {
                set = this.sets[index];
            }

            return set.Enqueue(state);
        }

        private readonly List<DeterministicSet> sets;
    }
}