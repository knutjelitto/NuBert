using System.Collections.Generic;
using Lingu.Terminals;

namespace Lingu.Automata
{
    public class DfaState
    {
        private DfaState(int StateId, bool isFinal)
        {
            this.StateId = StateId;
            IsFinal = isFinal;
            this.transitions = new List<DfaTransition>();
        }

        public int StateId { get; }
        public bool IsFinal { get; }

        public IReadOnlyCollection<DfaTransition> Transitions => this.transitions;

        public static DfaState Make(int stateNo, bool isFinal)
        {
            return new DfaState(stateNo, isFinal);
        }

        public static DfaState Inner(int stateNo)
        {
            return Make(stateNo, false);
        }

        public static DfaState Final(int stateNo)
        {
            return Make(stateNo, true);
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
            return StateId.ToString();
        }

        private readonly List<DfaTransition> transitions;
    }
}