using System.Collections.Generic;
using System.Linq;
using Lingu.Terminals;
using Lingu.Tools;

namespace Lingu.Automata
{
    public class NfaState
    {
        public NfaState()
        {
            this.transitions = new List<NfaTransition>();
        }

        public IEnumerable<TerminalNfaTransition> TerminalTransitions => this.transitions.OfType<TerminalNfaTransition>();
        public IEnumerable<EpsilonNfaTransition> EpsilonTransitions => this.transitions.OfType<EpsilonNfaTransition>();

        public IEnumerable<NfaTransition> Transitions => TerminalTransitions.Cast<NfaTransition>().Concat(EpsilonTransitions);

        public void Add(NfaState target)
        {
            Add(new EpsilonNfaTransition(target));
        }

        public void Add(Terminal terminal, NfaState target)
        {
            Add(new TerminalNfaTransition(terminal, target));
        }

        private void Add(NfaTransition transition)
        {
            this.transitions.Add(transition);
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

        private readonly List<NfaTransition> transitions;
    }
}