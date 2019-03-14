using System.Collections.Generic;
using Pliant.Collections;
using Pliant.Terminals;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class SubsetConstructionAlgorithm : NfaToDfa
    {
        public override DfaState Transform(Nfa nfa)
        {
            var processOnceQueue = new ProcessOnceQueue<NfaClosure>();

            var set = ObjectPoolExtensions.Allocate(SharedPools.Default<HashSet<NfaState>>());
            foreach (var state in nfa.Start.Closure())
            {
                set.Add(state);
            }

            var start = new NfaClosure(set, nfa.Start.Equals(nfa.End));

            processOnceQueue.Enqueue(start);

            while (processOnceQueue.Count > 0)
            {
                var nfaClosure = processOnceQueue.Dequeue();
                var transitions = ObjectPoolExtensions.Allocate(SharedPools.Default<Dictionary<AtomTerminal, HashSet<NfaState>>>());

                foreach (var state in nfaClosure.Set)
                {
                    foreach (var transition in state.Transitions)
                    {
                        if (transition is TerminalNfaTransition terminalTransition)
                        {
                            var terminal = terminalTransition.Terminal;

                            if (!transitions.ContainsKey(terminal))
                            {
                                transitions[terminal] = ObjectPoolExtensions.Allocate(SharedPools.Default<HashSet<NfaState>>());
                            }

                            transitions[terminal].Add(transition.Target);
                        }
                    }
                }

                foreach (var terminal in transitions.Keys)
                {
                    var targetStates = transitions[terminal];
                    var closure = Closure(targetStates, nfa.End);
                    closure = processOnceQueue.EnqueueOrGetExisting(closure);
                    nfaClosure.State.AddTransition(terminal, closure.State);
                    SharedPools.Default<HashSet<NfaState>>().ClearAndFree(targetStates);
                }

                SharedPools
                    .Default<HashSet<NfaState>>()
                    .ClearAndFree(nfaClosure.Set);
                SharedPools
                    .Default<Dictionary<AtomTerminal, HashSet<NfaState>>>()
                    .ClearAndFree(transitions);
            }

            return start.State;
        }

        private static NfaClosure Closure(IEnumerable<NfaState> states, NfaState endState)
        {
            var set = ObjectPoolExtensions.Allocate(SharedPools.Default<HashSet<NfaState>>());
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

        private class NfaClosure
        {
            public NfaClosure(HashSet<NfaState> closure, bool isFinal)
            {
                Set = closure;
                State = isFinal ? DfaState.Final() : DfaState.Inner();
                this._hashCode = HashCode.Compute(Set);
            }

            public HashSet<NfaState> Set { get; }

            public DfaState State { get; }

            public override bool Equals(object obj)
            {
                return obj is NfaClosure other && Set.SetEquals(other.Set);
            }

            public override int GetHashCode()
            {
                return this._hashCode;
            }

            private readonly int _hashCode;
        }
    }
}