using System.Collections;
using System.Collections.Generic;
using Pliant.Dotted;

namespace Pliant.Charts
{
    public class NormalStateList : IReadOnlyList<StateBase>
    {
        public int Count => this.list.Count;

        public StateBase this[int index] => this.list[index];

        public bool AddUnique(StateBase stateImplementation)
        {
            if (this.lookup.TryGetValue(stateImplementation.DottedRule, out var origins))
            {
                if (origins.TryGetValue(stateImplementation.Origin, out var _))
                {
                    return false;
                }
            }
            else
            {
                origins = new Dictionary<int, int>();
                this.lookup.Add(stateImplementation.DottedRule, origins);
            }

            origins.Add(stateImplementation.Origin, this.list.Count);
            this.list.Add(stateImplementation);

            return true;
        }

        public bool Contains(DottedRule rule, int origin)
        {
            return this.lookup.TryGetValue(rule, out var origins) && origins.TryGetValue(origin, out var _);
        }

        public IEnumerator<StateBase> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Dictionary<DottedRule, Dictionary<int, int>> lookup = new Dictionary<DottedRule, Dictionary<int, int>>();

        private readonly List<StateBase> list = new List<StateBase>();
    }
}