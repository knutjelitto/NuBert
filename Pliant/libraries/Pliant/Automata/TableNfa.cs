using Pliant.Collections;
using Pliant.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Automata
{
    public class TableNfa
    {
        private readonly Dictionary<int, Dictionary<char, int>> _table;
        private readonly HashSet<int> _finalStates;
        private readonly Dictionary<int, UniqueList<int>> _nullTransitions;

        public TableNfa(int start)
        {
            Start = start;
            this._table = new Dictionary<int, Dictionary<char, int>>();
            this._finalStates = new HashSet<int>();
            this._nullTransitions = new Dictionary<int, UniqueList<int>>();
        }

        public void AddTransition(int source, char character, int target)
        {
            var sourceTransitions = this._table.AddOrGetExisting(source);
            sourceTransitions[character] = target;
        }

        public void AddNullTransition(int source, int target)
        {
            this._nullTransitions
                .AddOrGetExisting(source)
                .AddUnique(target);
        }

        public int Start { get; private set; }

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

        public TableDfa ToDfa()
        {
            var queuePool = SharedPools.Default<ProcessOnceQueue<Closure>>();
            var queue = queuePool.Allocate();
            queue.Clear();
            var start = new Closure(Start, this._nullTransitions, this._finalStates);

            queue.Enqueue(
                start);

            var tableDfa = new TableDfa(start.GetHashCode());

            while (queue.Count > 0)
            {
                var transitions = SharedPools
                       .Default<Dictionary<char, SortedSet<int>>>()
                       .AllocateAndClear();

                var nfaClosure = queue.Dequeue();
                var nfaClosureId = nfaClosure.GetHashCode();
                tableDfa.SetFinal(nfaClosureId, nfaClosure.IsFinal);

                for (var i = 0; i < nfaClosure.States.Length; i++)
                {
                    var state = nfaClosure.States[i];
                    if (!this._table.TryGetValue(state, out var characterTransitions))
                    {
                        continue;
                    }

                    foreach (var characterTransition in characterTransitions)
                    {
                        if (!transitions.TryGetValue(characterTransition.Key, out var targets))
                        {
                            targets = SharedPools.Default<SortedSet<int>>().AllocateAndClear();
                            transitions.Add(characterTransition.Key, targets);
                        }

                        targets.Add(characterTransition.Value);
                    }
                }

                foreach (var targetSet in transitions)
                {
                    var closure = new Closure(targetSet.Value, this._nullTransitions, this._finalStates);
                    closure = queue.EnqueueOrGetExisting(closure);
                    var closureId = closure.GetHashCode();

                    tableDfa.AddTransition(nfaClosureId, targetSet.Key, closureId);
                    tableDfa.SetFinal(closureId, closure.IsFinal);

                    SharedPools.Default<SortedSet<int>>().ClearAndFree(targetSet.Value);
                }

                SharedPools
                       .Default<Dictionary<char, SortedSet<int>>>()
                       .ClearAndFree(transitions);
            }

            queuePool.Free(queue);
            return tableDfa;
        }

        private class Closure
        {
            private readonly SortedSet<int> _set;

            private readonly int _hashCode;

            public int[] States { get; private set; }

            public bool IsFinal { get; private set; }

            public Closure(
                SortedSet<int> sources,
                Dictionary<int, UniqueList<int>> nullTransitions,
                HashSet<int> finalStates)
            {
                this._set = sources;
                var queue = new ProcessOnceQueue<int>();
                foreach (var item in sources)
                {
                    queue.Enqueue(item);
                }

                CreateClosure(nullTransitions, finalStates, queue);
                this._hashCode = ComputeHashCode(States);
            }

            public Closure(
                int source, 
                Dictionary<int, UniqueList<int>> nullTransitions,
                HashSet<int> finalStates)
            {
                this._set = new SortedSet<int>();
                var queue = new ProcessOnceQueue<int>();
                queue.Enqueue(source);
                CreateClosure(nullTransitions, finalStates, queue);
                this._hashCode = ComputeHashCode(States);
            }

            private void CreateClosure(Dictionary<int, UniqueList<int>> nullTransitions, HashSet<int> finalStates, ProcessOnceQueue<int> queue)
            {
                while (queue.Count > 0)
                {
                    var state = queue.Dequeue();
                    this._set.Add(state);
                    if (finalStates.Contains(state))
                    {
                        IsFinal = true;
                    }

                    if (!nullTransitions.TryGetValue(state, out var targetStates))
                    {
                        continue;
                    }

                    for (var i = 0; i < targetStates.Count; i++)
                    {
                        queue.Enqueue(targetStates[i]);
                    }
                }
                States = this._set.ToArray();
            }

            private static int ComputeHashCode(int[] states)
            {
                var hashCode = 0;
                for (var i = 0; i < states.Length; i++)
                {
                    hashCode = HashCode.ComputeIncrementalHash(states[i].GetHashCode(), hashCode, i == 0);
                }
                return hashCode;
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                var closure = obj as Closure;
                if (closure == null)
                {
                    return false;
                }

                for (var i = 0; i < States.Length; i++)
                {
                    if (!closure._set.Contains(States[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
