namespace Lingu.Automata
{
    public class DfaTransition
    {
        public DfaTransition(Atom terminal, DfaState target)
        {
            Terminal = terminal;
            Target = target;
        }

        public Atom Terminal { get; }
        public DfaState Target { get; private set; }

        public void Retarget(DfaState newTarget)
        {
            Target = newTarget;
        }
    }
}