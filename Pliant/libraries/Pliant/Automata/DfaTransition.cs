using Pliant.Grammars;

namespace Pliant.Automata
{
    public class DfaTransition
    {
        public DfaTransition(Terminal terminal, DfaState target)
        {
            Target = target;
            Terminal = terminal;
        }

        public DfaState Target { get; }
        public Terminal Terminal { get; }
    }
}