using Lingu.Terminals;

namespace Lingu.Automata
{
    public class TerminalNfaTransition : NfaTransition
    {
        public TerminalNfaTransition(Terminal terminal, NfaState target)
            : base(target)
        {
            Terminal = terminal;
        }

        public Terminal Terminal { get; }
    }
}