namespace Pliant.Automata
{
    public abstract class NfaTransition
    {
        protected NfaTransition(NfaState target)
        {
            Target = target;
        }

        public NfaState Target { get; }
    }
}