using System.Collections.Generic;
using Pliant.Collections;
using System;
using System.Linq;
using Pliant.Utilities;

namespace Pliant.Automata
{
    public class NfaState
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

        public override bool Equals(object obj)
        {
            return obj is NfaState other && Transitions.SequenceEqual(other.Transitions);
        }

        public override int GetHashCode()
        {
            return HashCode.Compute(Transitions);
        }
    }    
}