using System.Collections.Generic;
using Lingu.Terminals;

namespace Lingu.Automata
{
    public class DfaState
    {
        private DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            this.transitions = new List<DfaTransition>();
        }

        public bool IsFinal { get; }

        public IReadOnlyCollection<DfaTransition> Transitions => this.transitions;

        public static DfaState Make(bool isFinal)
        {
            return new DfaState(isFinal);
        }

        public static DfaState Inner()
        {
            return Make(false);
        }

        public static DfaState Final()
        {
            return Make(true);
        }

        private void Add(DfaTransition transition)
        {
            this.transitions.Add(transition);
        }

        public void Add(Terminal terminal, DfaState target)
        {
            Add(new DfaTransition(terminal, target));
        }

        public override string ToString()
        {
            return $"({IsFinal},({string.Join(",", this.transitions)}))";
        }

        private readonly List<DfaTransition> transitions;
    }
}