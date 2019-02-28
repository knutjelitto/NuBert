using System.Collections;
using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public class NormalStateList : IReadOnlyList<NormalState>
    {
        public int Count => this.list.Count;

        public bool AddUnique(NormalState normalState)
        {
            if (this.index.TryGetValue(normalState.DottedRule, out var origins))
            {
                if (origins.TryGetValue(normalState.Origin, out var _))
                {
                    return false;
                }
            }
            else
            {
                origins = new Dictionary<int, int>();
                this.index.Add(normalState.DottedRule, origins);
            }

            origins.Add(normalState.Origin, this.list.Count);
            this.list.Add(normalState);

            return true;
        }

        public bool Contains(DottedRule rule, int origin)
        {
            return this.index.TryGetValue(rule, out var origins) && origins.TryGetValue(origin, out var _);
        }

        public IEnumerator<NormalState> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Dictionary<DottedRule, Dictionary<int, int>> index = new Dictionary<DottedRule, Dictionary<int, int>>();

        private readonly List<NormalState> list = new List<NormalState>();

        #region  not sortable (modify ReSharper template to catch these cases)

        public NormalState this[int index]
        {
            get => this.list[index];
            set => this.list[index] = value;
        }

        #endregion
    }
}