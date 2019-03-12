using System.Collections.Generic;
using System.Linq;
using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Terminals;

namespace Pliant.Automata
{
    public class NfaState
    {
        public NfaState()
        {
            this._transitions = new List<NfaTransition>();
        }

        public IReadOnlyList<NfaTransition> Transitions => this._transitions;

        public void AddEpsilon(NfaState target)
        {
            AddTransition(new EpsilonNfaTransition(target));
        }

        public void AddTransition(AtomTerminal terminal, NfaState target)
        {
            AddTransition(new TerminalNfaTransition(terminal, target));
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
                foreach (var transition in state.Transitions.OfType<EpsilonNfaTransition>())
                {
                    queue.Enqueue(transition.Target);
                }
            }

            return queue.Visited;
        }

        private void AddTransition(NfaTransition transition)
        {
            this._transitions.Add(transition);
        }

        private readonly List<NfaTransition> _transitions;
    }
}