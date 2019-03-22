using Lingu.Terminals;
using Lingu.Tools;

namespace Lingu.Automata
{
    public static class DfaConverter
    {
        public static Dfa Convert(Nfa nfa)
        {
            var once = new UniqueQueue<NfaClosure>();
            var start = new NfaClosure(nfa.Start, nfa.End);
            once.Enqueue(start);

            while (once.Count > 0)
            {
                var closure = once.Dequeue();
                var transitions = closure.UnambiguateTransitions();

                foreach (var transition in transitions)
                {
                    var terminal = transition.Key;
                    var targets = transition.Value;
                    var targetClosure = new NfaClosure(targets, nfa.End);
                    once.Enqueue(targetClosure, out targetClosure);
                    var target = targetClosure.State;

                    closure.State.Add(Terminal.From(terminal), target);
                }
            }

            //return new Dfa(start.State);
            return new Dfa(start.State);
        }
    }
}