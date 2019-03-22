using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lingu.Automata
{
    public class DfaPlumber
    {
        public DfaPlumber(Dfa dfa)
        {
            SetDfa(dfa);
        }

        protected void SetDfa(Dfa dfa)
        {
            Dfa = dfa;
            this.states = VisitStates();
        }

        protected Dfa Dfa { get; private set; }

        public int StateCount => this.states.Count;

        public void Dump(TextWriter writer)
        {
            foreach (var pair in this.states.OrderBy(s => s.Value))
            {
                var finA = pair.Key.IsFinal ? "(" : ".";
                var finB = pair.Key.IsFinal ? ")" : ".";
                writer.WriteLine($"{finA}{pair.Value}{finB}:");
                foreach (var transition in pair.Key.Transitions)
                {
                    writer.WriteLine($"  {this.states[transition.Target]} <= {transition.Terminal}");
                }
            }
        }

        private Dictionary<DfaState, int> VisitStates()
        {
            var visited = new Dictionary<DfaState, int>();

            void Visit(DfaState state)
            {
                if (!visited.TryGetValue(state, out var _))
                {
                    visited.Add(state, visited.Count + 1);

                    foreach (var transition in state.Transitions)
                    {
                        Visit(transition.Target);
                    }
                }
            }

            Visit(Dfa.Start);

            return visited;
        }

        protected Dictionary<DfaState, int> states;
    }
}