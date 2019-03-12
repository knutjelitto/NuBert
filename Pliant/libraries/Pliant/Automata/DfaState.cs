using System.Collections.Generic;
using Pliant.Terminals;

namespace Pliant.Automata
{
    public struct DfaState
    {
        private DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            this._transitions = new List<DfaTransition>();
        }

        public static DfaState Inner()
        {
            return new DfaState(false);
        }

        public static DfaState Final()
        {
            return new DfaState(true);
        }

        public bool IsFinal { get; }

        public IReadOnlyList<DfaTransition> Transitions => this._transitions;

        public DfaState AddTransition(AtomTerminal terminal, DfaState target)
        {
            this._transitions.Add(new DfaTransition(terminal, target));
            return this;
        }

        private readonly List<DfaTransition> _transitions;
    }
}