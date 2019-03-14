using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lingu.Terminals;
using Lingu.Tools;

namespace Lingu.Automata
{
    public static class Transform
    {
        public static Dfa ToDfa(this Nfa nfa)
        {
            var once = new UniqueQueue<NfaClosure>();
            var stateNo = 1;
            var start = new NfaClosure(stateNo++, nfa.Start, nfa.End);
            once.Enqueue(start);

            while (once.Count > 0)
            {
                var closure = once.Dequeue();
                var transitions = new Dictionary<Terminal, HashSet<NfaState>>();

                foreach (var state in closure.Set)
                {
                    foreach (var transition in state.TerminalTransitions)
                    {
                        if (!transitions.TryGetValue(transition.Terminal, out var set))
                        {
                            set = new HashSet<NfaState>();
                            transitions.Add(transition.Terminal, set);
                        }

                        set.Add(transition.Target);
                    }
                }

                CharUnicodeInfo.GetUnicodeCategory('a');

                foreach (var transition in transitions)
                {
                    var terminal = transition.Key;
                    var targets = transition.Value;
                    var targetClosure = new NfaClosure(stateNo, targets, nfa.End);
                    if (once.Enqueue(targetClosure, out targetClosure))
                    {
                        stateNo++;
                    }
                    var target = targetClosure.State;

                    closure.State.Add(terminal, target);
                }
            }

            return new Dfa(start.State);
        }
    }
}
