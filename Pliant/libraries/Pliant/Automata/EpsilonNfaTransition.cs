namespace Pliant.Automata
{
    public sealed class EpsilonNfaTransition : NfaTransition
    {
        public EpsilonNfaTransition(NfaState target)
            : base(target)
        {
        }
    }
}