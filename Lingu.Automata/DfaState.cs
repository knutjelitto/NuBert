using System.Collections.Generic;

namespace Lingu.Automata
{
    public class DfaState
    {
        public DfaState(bool isFinal)
        {
            IsFinal = isFinal;
            Transitions = new List<DfaTransition>();
        }

        public bool IsFinal { get; }

        public List<DfaTransition> Transitions { get; }

        public static DfaState Make(bool isFinal)
        {
            return new DfaState(isFinal);
        }

        private void Add(DfaTransition transition)
        {
            Transitions.Add(transition);
        }

        public void Add(Atom terminal, DfaState target)
        {
            Add(new DfaTransition(terminal, target));
        }

        public override string ToString()
        {
            return $"({IsFinal},({string.Join(",", Transitions)}))";
        }
    }
}