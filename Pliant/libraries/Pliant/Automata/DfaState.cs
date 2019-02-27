using System.Collections.Generic;

namespace Pliant.Automata
{
    public class DfaState
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

        public void AddTransition(DfaTransition edge)
        {
            this._transitions.Add(edge);
        }

        private readonly List<DfaTransition> _transitions;
    }
}