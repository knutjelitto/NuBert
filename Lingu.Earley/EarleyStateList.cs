using System.Collections;
using System.Collections.Generic;

namespace Lingu.Earley
{
    public class EarleyStateList<T> : IReadOnlyList<T>
        where T : EarleyState
    {
        public int Count => this.list.Count;

        public T this[int index] => this.list[index];

        public bool Add(T state)
        {
            if (this.lookup.TryGetValue(state.DottedRule, out var origins))
            {
                if (origins.TryGetValue(state.Origin, out var _))
                {
                    return false;
                }
            }
            else
            {
                origins = new Dictionary<int, int>();
                this.lookup.Add(state.DottedRule, origins);
            }

            origins.Add(state.Origin, this.list.Count);
            this.list.Add(state);

            return true;
        }

        public bool Contains(DottedRule rule, int origin)
        {
            return this.lookup.TryGetValue(rule, out var origins) && origins.TryGetValue(origin, out var _);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<T> list = new List<T>();

        private readonly Dictionary<DottedRule, Dictionary<int, int>> lookup = new Dictionary<DottedRule, Dictionary<int, int>>();
    }
}