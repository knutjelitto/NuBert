using System.Collections.Generic;
using System.Linq;
using Lingu.Commons;

namespace Lingu.Automata
{
    public class NfaState
    {
        public NfaState()
        {
            this.transitions = new List<NfaTransition>();
        }

        public IEnumerable<NfaTransition> EpsilonTransitions => this.transitions.Where(t => t.Terminal.Set.IsEmpty);

        public IEnumerable<NfaTransition> TerminalTransitions => this.transitions.Where(t => !t.Terminal.Set.IsEmpty);

        public IEnumerable<NfaTransition> Transitions => TerminalTransitions.Concat(EpsilonTransitions);

        public void Add(NfaState target)
        {
            Add(new NfaTransition(Atom.From(IntegerSet.Empty), target));
        }

        public void Add(Atom terminal, NfaState target)
        {
            Add(new NfaTransition(terminal, target));
        }

        public IEnumerable<NfaState> Closure()
        {
            var once = new UniqueQueue<NfaState>();

            once.Enqueue(this);

            while (once.Count > 0)
            {
                var state = once.Dequeue();
                foreach (var transition in state.EpsilonTransitions)
                {
                    once.Enqueue(transition.Target);
                }
            }

            return once.Seen;
        }

        private void Add(NfaTransition transition)
        {
            this.transitions.Add(transition);
        }

        private readonly List<NfaTransition> transitions;
    }
}