namespace Lingu.Automata
{
    public class Dfa
    {
        public Dfa(DfaState start)
        {
            Start = start;
        }

        public DfaState Start { get; }
    }
}