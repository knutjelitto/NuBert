namespace Lingu.Automata
{
    public class NfaTransition
    {
        public NfaTransition(Atom terminal, NfaState target)
        {
            Terminal = terminal;
            Target = target;
        }

        public Atom Terminal { get; }
        public NfaState Target { get; }
    }
}