using Pliant.Terminals;

namespace Pliant.Automata
{
    public sealed class DfaTransition
    {
        public DfaTransition(AtomTerminal terminal, DfaState target)
        {
            Target = target;
            Terminal = terminal;
        }

        public DfaState Target { get; }
        public AtomTerminal Terminal { get; }
    }
}