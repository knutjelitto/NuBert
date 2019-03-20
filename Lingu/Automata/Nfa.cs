using System.Collections.Generic;
using Lingu.Tools;

namespace Lingu.Automata
{
    public class Nfa
    {
        public Nfa(NfaState start, NfaState end)
        {
            Start = start;
            End = end;
        }

        public NfaState End { get; }
        public NfaState Start { get; }

        public Dfa ToDfa()
        {
            var once = new UniqueQueue<NfaClosure>();
            var start = new NfaClosure(Start, End);
            once.Enqueue(start);

            while (once.Count > 0)
            {
                var closure = once.Dequeue();
                var transitions = closure.UnambiguateTransitions();

                foreach (var transition in transitions)
                {
                    var terminal = transition.Key;
                    var targets = transition.Value;
                    var targetClosure = new NfaClosure(targets, End);
                    once.Enqueue(targetClosure, out targetClosure);
                    var target = targetClosure.State;

                    closure.State.Add(terminal, target);
                }
            }

            return new Dfa(start.State);
        }

        public Nfa Clone()
        {
            var map = new Dictionary<NfaState, NfaState>();

            return new Nfa(Map(Start, map), Map(End, map));
        }

        private NfaState Map(NfaState state, Dictionary<NfaState, NfaState> map)
        {
            if (!map.TryGetValue(state, out var mapped))
            {
                mapped = new NfaState();
                map.Add(state, mapped);

                foreach (var transition in state.TerminalTransitions)
                {
                    mapped.Add(transition.Terminal, Map(transition.Target, map));
                }
                foreach (var transition in state.EpsilonTransitions)
                {
                    mapped.Add(Map(transition.Target, map));
                }
            }

            return mapped;
        }
    }
}