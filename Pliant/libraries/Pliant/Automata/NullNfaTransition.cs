namespace Pliant.Automata
{
    public sealed class NullNfaTransition : NfaTransition
    {
        public NullNfaTransition(NfaState target)
            : base(target)
        {
        }
    }
}