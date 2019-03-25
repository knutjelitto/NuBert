using Lingu.Terminals;

namespace Lingu.Automata
{
    public class NfaTransition
    {
        public NfaTransition(Terminal terminal, NfaState target)
        {
            Terminal = terminal;
            Target = target;
        }

        public Terminal Terminal { get; }
        public NfaState Target { get; }
    }
}