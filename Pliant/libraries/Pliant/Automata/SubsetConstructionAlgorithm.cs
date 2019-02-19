using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var set = new NfaStateSet();
            foreach (var state in nfa.Start.Closure())
            {
                set.Add(state);
            }

            var start = new NfaClosure(set, nfa.Start.Equals(nfa.End));

            processOnceQueue.Enqueue(start);

            while (processOnceQueue.Count > 0)
            {
                var nfaClosure = processOnceQueue.Dequeue();
                var transitions = new Dictionary<Terminal, NfaStateSet>();

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
                                    transitions[terminal] = new NfaStateSet();
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
                    nfaClosure.State.AddTransition(new DfaTransition(terminal, closure.State));
                }
            }

            return start.State;
        }

        private static NfaClosure Closure(NfaStateSet states, NfaState endState)
        {
            var set = new NfaStateSet();
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

        private class NfaStateSet : HashSet<NfaState>
        {
        }

        private class NfaClosure
        {
            public NfaClosure(NfaStateSet closure, bool isFinal)
            {
                Closure = closure.ToArray();
                State = new DfaState(isFinal);
                this._hashCode = HashCode.Compute(Closure);
            }

            public NfaState[] Closure { get; }

            public DfaState State { get; }

            public override bool Equals(object obj)
            {
#if true
                if (obj is NfaClosure other)
                {
                    if (this._hashCode == other._hashCode)
                    {
                        Debug.Assert(Closure.SequenceEqual(other.Closure));
                    }

                    if (Closure.SequenceEqual(other.Closure))
                    {
                        Debug.Assert(this._hashCode == other._hashCode);
                    }

                    //Debug.Assert(this._hashCode == HashCode.Compute(Closure));

                    //return other._hashCode.Equals(this._hashCode);
                    return Closure.SequenceEqual(other.Closure);
                }

                return false;
#else
                return obj is NfaClosure other && other._hashCode.Equals(this._hashCode);
#endif
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