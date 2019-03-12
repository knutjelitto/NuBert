using Pliant.Terminals;

namespace Pliant.Automata
{
    public sealed class TerminalNfaTransition : NfaTransition
    {
        public TerminalNfaTransition(AtomTerminal terminal, NfaState target)
            : base(target)
        {
            Terminal = terminal;
        }

        public AtomTerminal Terminal { get; }
    }
}