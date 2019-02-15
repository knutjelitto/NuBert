using System.Collections.Generic;
using Pliant.Collections;
using System;

namespace Pliant.Automata
{
    public class NfaState : INfaState, IComparable<INfaState>, IComparable
    {
        private readonly List<INfaTransition> _transitions;

        public NfaState()
        {
            this._transitions = new List<INfaTransition>();
        }

        public IReadOnlyList<INfaTransition> Transitions => this._transitions;

        public void AddTransistion(INfaTransition transition)
        {
            this._transitions.Add(transition);
        }

        public IEnumerable<INfaState> Closure()
        {
            // the working queue used to process states 
            var queue = new ProcessOnceQueue<INfaState>();
            
            // initialize by adding the curren state (this)
            queue.Enqueue(this);

            // loop over items in the queue, adding newly discovered
            // items after null transitions
            while (queue.Count != 0)
            {
                var state = queue.Dequeue();
                for (var t = 0; t < state.Transitions.Count; t++)
                {
                    var transition = state.Transitions[t];
                    if (transition.TransitionType == NfaTransitionType.Null)
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

            var nfaState = obj as INfaState;
            if (nfaState == null)
            {
                throw new ArgumentException("parameter must be a INfaState", nameof(obj));
            }

            return CompareTo(nfaState);
        }

        public int CompareTo(INfaState other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }
    }    
}