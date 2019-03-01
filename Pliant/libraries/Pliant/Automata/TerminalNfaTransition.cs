using Pliant.Terminals;

namespace Pliant.Automata
{
    public sealed class TerminalNfaTransition : NfaTransition
    {
        public TerminalNfaTransition(Terminal terminal, NfaState target)
            : base(target)
        {
            Terminal = terminal;
        }

        public Terminal Terminal { get; }
    }
}