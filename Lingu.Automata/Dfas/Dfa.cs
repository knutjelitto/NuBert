using System.Collections.Generic;
using System.IO;

namespace Lingu.Automata
{
    public class Dfa
    {
        public Dfa(DfaState start)
        {
            Start = start;
        }

        public DfaState Start { get; }

        public Dfa Minimize()
        {
            return new DfaMinimizer(this).Minimize();
        }

        public void Dump(TextWriter writer)
        {
            new DfaPlumber(this).Dump(writer);
        }

        public Dfa Clone()
        {
            var map = new Dictionary<DfaState, DfaState>();

            DfaState Map(DfaState state)
            {
                if (!map.TryGetValue(state, out var mapped))
                {
                    mapped = new DfaState(state.IsFinal);
                    map.Add(state, mapped);

                    foreach (var transition in state.Transitions)
                    {
                        mapped.Add(transition.Terminal, Map(transition.Target));
                    }
                }

                return mapped;
            }

            return new Dfa(Map(Start));
        }

    }
}