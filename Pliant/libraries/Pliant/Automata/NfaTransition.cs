namespace Pliant.Automata
{
    public abstract class NfaTransition //: INfaTransition
    {
        protected NfaTransition(NfaState target)
        {
            Target = target;
        }

        public NfaState Target { get; }

        public abstract NfaTransitionType TransitionType { get; }
    }
}