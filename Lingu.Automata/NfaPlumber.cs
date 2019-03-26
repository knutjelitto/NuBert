using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lingu.Automata
{
    public class NfaPlumber
    {
        private readonly Dictionary<NfaState, int> states;

        public NfaPlumber(Nfa nfa)
        {
            Nfa = nfa;

            this.states = VisitStates();
        }

        public Nfa Nfa { get; }


        public void Dump(TextWriter writer)
        {
            foreach (var pair in this.states.OrderBy(s => s.Value))
            {
                var final = pair.Key.Equals(Nfa.End);

                var finA = final ? "(" : ".";
                var finB = final ? ")" : ".";
                writer.WriteLine($"{finA}{pair.Value}{finB}:");
                foreach (var transition in pair.Key.TerminalTransitions)
                {
                    writer.WriteLine($"  {this.states[transition.Target]} <= {transition.Terminal}");
                }
                foreach (var transition in pair.Key.EpsilonTransitions)
                {
                    writer.WriteLine($"  {this.states[transition.Target]} <= -epsilon-");
                }
            }
        }

        private Dictionary<NfaState, int> VisitStates()
        {
            var visited = new Dictionary<NfaState, int>();

            void Visit(NfaState state)
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

            Visit(Nfa.Start);

            return visited;
        }
    }
}