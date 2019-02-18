using System;
using System.Collections.Generic;
using System.Linq;
using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class SubsetConstructionAlgorithm : INfaToDfa
    {
        public DfaState Transform(Nfa nfa)
        {
            var processOnceQueue = new ProcessOnceQueue<NfaClosure>();

            var set = SharedPools.Default<SortedSet<NfaState>>().AllocateAndClear();
            foreach (var state in nfa.Start.Closure())
            {
                set.Add(state);
            }

            var start = new NfaClosure(set, nfa.Start.Equals(nfa.End));

            processOnceQueue.Enqueue(start);

            while (processOnceQueue.Count > 0)
            {
                var nfaClosure = processOnceQueue.Dequeue();
                var transitions = SharedPools
                                  .Default<Dictionary<Terminal, SortedSet<NfaState>>>()
                                  .AllocateAndClear();

                for (var i = 0; i < nfaClosure.Closure.Length; i++)
                {
                    var state = nfaClosure.Closure[i];
                    for (var t = 0; t < state.Transitions.Count; t++)
                    {
                        var transition = state.Transitions[t];
                        switch (transition)
                        {
                            case TerminalNfaTransition terminalTransition:
                                var terminal = terminalTransition.Terminal;

                                if (!transitions.ContainsKey(terminalTransition.Terminal))
                                {
                                    transitions[terminal] = SharedPools.Default<SortedSet<NfaState>>().AllocateAndClear();
                                }

                                transitions[terminal].Add(transition.Target);
                                break;
                        }
                    }
                }

                foreach (var terminal in transitions.Keys)
                {
                    var targetStates = transitions[terminal];
                    var closure = Closure(targetStates, nfa.End);
                    closure = processOnceQueue.EnqueueOrGetExisting(closure);
                    nfaClosure.State.AddTransition(
                        new DfaTransition(terminal, closure.State));
                    SharedPools.Default<SortedSet<NfaState>>().ClearAndFree(targetStates);
                }

                SharedPools
                    .Default<SortedSet<NfaState>>()
                    .ClearAndFree(nfaClosure.Set);
                SharedPools
                    .Default<Dictionary<Terminal, SortedSet<NfaState>>>()
                    .ClearAndFree(transitions);
            }

            return start.State;
        }

        private static NfaClosure Closure(SortedSet<NfaState> states, NfaState endState)
        {
            var set = SharedPools.Default<SortedSet<NfaState>>().AllocateAndClear();
            var isFinal = false;
            foreach (var state in states)
            {
                foreach (var item in state.Closure())
                {
                    if (item.Equals(endState))
                    {
                        isFinal = true;
                    }

                    set.Add(item);
                }
            }

            return new NfaClosure(set, isFinal);
        }

        #region  not sortable (modify ReSharper template to catch these cases)

        private class NfaClosure : IComparable<NfaClosure>, IComparable
        {
            public NfaClosure(SortedSet<NfaState> closure, bool isFinal)
            {
                Set = closure;
                this._hashCode = HashCode.Compute(closure);
                Closure = closure.ToArray();
                State = new DfaState(isFinal);
            }

            public NfaState[] Closure { get; }

            public SortedSet<NfaState> Set { get; }

            public DfaState State { get; }

            public int CompareTo(object obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException();
                }

                if (!(obj is NfaClosure nfaClosure))
                {
                    throw new ArgumentException("instance of NfaClosure expected.", nameof(obj));
                }

                return CompareTo(nfaClosure);
            }

            public int CompareTo(NfaClosure other)
            {
                return GetHashCode().CompareTo(other.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                if (!(obj is NfaClosure nfaClosure))
                {
                    return false;
                }

                return nfaClosure._hashCode.Equals(this._hashCode);
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }

            private readonly int _hashCode;
        }

        #endregion
    }
}