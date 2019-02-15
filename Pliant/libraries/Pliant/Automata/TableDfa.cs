using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Automata
{
    public class TableDfa
    {
        private readonly Dictionary<int, Dictionary<char, int>> _table;
        private readonly HashSet<int> _finalStates;

        public TableDfa(int start)
        {
            Start = start;
            this._table = new Dictionary<int, Dictionary<char, int>>();
            this._finalStates = new HashSet<int>();
        }

        public void AddTransition(int source, char character, int target)
        {
            var sourceTransitions = this._table.AddOrGetExisting(source);
            sourceTransitions[character] = target;
        }

        public void SetFinal(int state, bool isFinal)
        {
            if (isFinal)
            {
                this._finalStates.Add(state);
            }
            else
            {
                this._finalStates.Remove(state);
            }
        }

        public bool IsFinal(int state)
        {
            return this._finalStates.Contains(state);
        }

        public int Start { get; private set; }

        public int? Transition(int source, char character)
        {
            if (!this._table.TryGetValue(source, out var sourceTransitions))
            {
                return null;
            }

            if (sourceTransitions.TryGetValue(character, out var target))
            {
                return target;
            }

            return null;
        }
    }
}
