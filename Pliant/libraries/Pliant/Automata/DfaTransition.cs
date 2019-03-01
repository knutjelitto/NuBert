using Pliant.Terminals;

namespace Pliant.Automata
{
    public sealed class DfaTransition
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