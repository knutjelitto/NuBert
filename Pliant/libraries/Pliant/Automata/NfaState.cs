using System.Collections.Generic;
using Pliant.Collections;
using System;
using Pliant.Grammars;

namespace Pliant.Automata
{
    public class NfaState : IComparable<NfaState>, IComparable
    {
        private readonly List<NfaTransition> _transitions;

        public NfaState()
        {
            this._transitions = new List<NfaTransition>();
        }

        public IReadOnlyList<NfaTransition> Transitions => this._transitions;

        public void AddEpsilon(NfaState target)
        {
            AddTransition(new NullNfaTransition(target));
        }

        public void AddTransition(Terminal terminal, NfaState target)
        {
            AddTransition(new TerminalNfaTransition(terminal, target));
        }

        private void AddTransition(NfaTransition transition)
        {
            this._transitions.Add(transition);
        }

        public IEnumerable<NfaState> Closure()
        {
            // the working queue used to process states 
            var queue = new ProcessOnceQueue<NfaState>();
            
            // initialize by adding the current state (this)
            queue.Enqueue(this);

            // loop over items in the queue, adding newly discovered
            // items after null transitions
            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                foreach (var transition in state.Transitions)
                {
                    if (transition is NullNfaTransition)
                    {
                        queue.Enqueue(transition.Target);
                    }
                }
            }

            return queue.Visited;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }

            if (!(obj is NfaState nfaState))
            {
                throw new ArgumentException("parameter must be a INfaState", nameof(obj));
            }

            return CompareTo(nfaState);
        }

        public int CompareTo(NfaState other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }
    }    
}