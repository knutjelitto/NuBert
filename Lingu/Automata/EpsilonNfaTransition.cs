namespace Lingu.Automata
{
    public class EpsilonNfaTransition : NfaTransition
    {
        public EpsilonNfaTransition(NfaState target)
            : base(target)
        {
        }
    }
}