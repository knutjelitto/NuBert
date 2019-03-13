namespace Lingu.Automata
{
    public class Nfa
    {
        public Nfa(NfaState start, NfaState end)
        {
            Start = start;
            End = end;
        }

        public NfaState End { get; private set; }
        public NfaState Start { get; }

        public void Append(Nfa other)
        {
            End.Add(other.Start);
            End = other.End;
        }

        public void Alternate(Nfa other)
        {
            Start.Add(other.Start);
            other.End.Add(End);
        }

        public void Optional()
        {
            Start.Add(End);
        }

        public void Plus()
        {
            End.Add(Start);
        }

        public void Star()
        {
            Start.Add(End);
            End.Add(Start);
        }
    }
}