using Pliant.Grammars;

namespace Pliant.Automata
{
    public class TerminalNfaTransition : INfaTransition
    {
        public INfaState Target { get; private set; }

        public ITerminal Terminal { get; private set; }

        public NfaTransitionType TransitionType => NfaTransitionType.Edge;

        public TerminalNfaTransition(ITerminal terminal, INfaState target)
        {
            Terminal = terminal;
            Target = target;
        }
    }
}