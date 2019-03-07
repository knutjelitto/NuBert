﻿using System.Collections;
using System.Collections.Generic;
using Pliant.Dotted;

namespace Pliant.Charts
{
    public class StateList<T> : IReadOnlyList<T>
        where T : RuleState
    {
        public int Count => this.list.Count;

        public T this[int index] => this.list[index];

        public bool AddUnique(T normalState)
        {
            if (this.lookup.TryGetValue(normalState.DottedRule, out var origins))
            {
                if (origins.TryGetValue(normalState.Origin, out var _))
                {
                    return false;
                }
            }
            else
            {
                origins = new Dictionary<int, int>();
                this.lookup.Add(normalState.DottedRule, origins);
            }

            origins.Add(normalState.Origin, this.list.Count);
            this.list.Add(normalState);

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

        private readonly Dictionary<DottedRule, Dictionary<int, int>> lookup = new Dictionary<DottedRule, Dictionary<int, int>>();

        private readonly List<T> list = new List<T>();
    }
}