namespace Pliant.Automata
{
    public abstract class NfaToDfa
    {
        public abstract DfaState Transform(Nfa nfa);
    }
}