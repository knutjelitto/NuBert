using Lingu.Terminals;

namespace Lingu.Automata
{
    public class DfaTransition
    {
        public DfaTransition(Terminal terminal, DfaState target)
        {
            Terminal = terminal;
            Target = target;
        }

        public Terminal Terminal { get; }
        public DfaState Target { get; }
    }
}