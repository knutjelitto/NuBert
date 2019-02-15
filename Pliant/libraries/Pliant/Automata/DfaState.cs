using System.Collections.Generic;

namespace Pliant.Automata
{
    public class DfaState : IDfaState
    {
        public DfaState()
            : this(false)
        {
        }

        public DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            this._transitions = new List<IDfaTransition>();
        }

        public bool IsFinal { get; }

        public IReadOnlyList<IDfaTransition> Transitions => this._transitions;

        public void AddTransition(IDfaTransition edge)
        {
            this._transitions.Add(edge);
        }

        private readonly List<IDfaTransition> _transitions;
    }
}