using System.Collections.Generic;
using System.Linq;
using Lingu.Terminals;
using Lingu.Tools;

namespace Lingu.Automata
{
    public class NfaClosure
    {
        public NfaClosure(NfaState state, NfaState end)
            : this(Enumerable.Repeat(state, 1), end)
        {
        }

        public NfaClosure(IEnumerable<NfaState> states, NfaState end)
        {
            var set = new HashSet<NfaState>();
            var isFinal = false;

            foreach (var state in states)
            {
                foreach (var closureState in state.Closure())
                {
                    if (closureState.Equals(end))
                    {
                        isFinal = true;
                    }

                    set.Add(closureState);
                }
            }

            Set = set;
            State = DfaState.Make(isFinal);
            this.hashCode = Set.AddedHash();
        }

        public Dictionary<Terminal, HashSet<NfaState>> UnambiguateTransitions()
        {
            var transitions = new Dictionary<Terminal, HashSet<NfaState>>();

            foreach (var state in Set)
            {
                foreach (var transition in state.TerminalTransitions)
                {
                    var newTerminal = transition.Terminal;

                    while (true)
                    {
                        var already = transitions.Keys.FirstOrDefault(terminal => terminal.Overlaps(newTerminal));
                        if (already != null)
                        {
                            transitions[already].Add(transition.Target);

                            if (already.AlmostEquals(transition.Terminal))
                            {
                                break;
                            }

                            newTerminal = transition.Terminal.ExceptWith(already);
                        }
                        else
                        {
                            transitions.Add(newTerminal, new HashSet<NfaState> { transition.Target });
                            break;
                        }
                    }
                }
            }

            return transitions;
        }

        public IEnumerable<TerminalNfaTransition> TerminalTransitions =>
            Set.SelectMany(state => state.TerminalTransitions);

        public HashSet<NfaState> Set { get; }

        public DfaState State { get; }

        public override bool Equals(object obj)
        {
            return obj is NfaClosure other && Set.SetEquals(other.Set);
        }

        public override int GetHashCode() => this.hashCode;

        private readonly int hashCode;
    }
}
