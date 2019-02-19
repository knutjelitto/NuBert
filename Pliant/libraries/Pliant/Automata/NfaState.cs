using System.Collections.Generic;
using Pliant.Collections;
using System;

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

        public void AddTransistion(NfaTransition transition)
        {
            this._transitions.Add(transition);
        }

        public IEnumerable<NfaState> Closure()
        {
            // the working queue used to process states 
            var queue = new ProcessOnceQueue<NfaState>();
            
            // initialize by adding the curren state (this)
            queue.Enqueue(this);

            // loop over items in the queue, adding newly discovered
            // items after null transitions
            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                for (var t = 0; t < state.Transitions.Count; t++)
                {
                    var transition = state.Transitions[t];
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