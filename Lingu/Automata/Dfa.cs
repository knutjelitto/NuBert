using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lingu.Automata
{
    public class Dfa
    {
        public Dfa(DfaState start)
        {
            Start = start;
        }

        public DfaState Start { get; }

        public int StateCount => VisitAll().Count;

        public void Dump(TextWriter writer)
        {
            var nums = VisitAll();

            var states = nums.OrderBy(p => p.Value);

            foreach (var pair in states)
            {
                var finA = pair.Key.IsFinal ? "(" : ".";
                var finB = pair.Key.IsFinal ? ")" : ".";
                writer.WriteLine($"{finA}{pair.Value}{finB}:");
                foreach (var transition in pair.Key.Transitions)
                {
                    writer.WriteLine($"  {nums[transition.Target]} <= {transition.Terminal}");
                }
            }
        }

        private Dictionary<DfaState, int> VisitAll()
        {
            var set = new Dictionary<DfaState, int>();
            Visit(set, Start);
            return set;
        }

        private void Visit(Dictionary<DfaState, int> already, DfaState state)
        {
            if (!already.TryGetValue(state, out var _))
            {
                already.Add(state, already.Count + 1);

                foreach (var transition in state.Transitions)
                {
                    Visit(already, transition.Target);
                }
            }
        }
    }
}