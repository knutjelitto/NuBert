namespace Pliant.Automata
{
    public class Nfa
    {
        public Nfa(NfaState start, NfaState end)
        {
            Start = start;
            End = end;
        }

        public NfaState End { get; }
        public NfaState Start { get; }

        public Nfa Concatenation(Nfa other)
        {
            End.AddEpsilon(other.Start);
            return this;
        }

        public Nfa Kleene()
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddEpsilon(Start);
            newStart.AddEpsilon(newEnd);

            newEnd.AddEpsilon(Start);
            End.AddEpsilon(newEnd);

            return new Nfa(newStart, newEnd);
        }

        public Nfa Union(Nfa other)
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddEpsilon(Start);
            newStart.AddEpsilon(other.Start);

            End.AddEpsilon(newEnd);
            other.End.AddEpsilon(newEnd);

            return new Nfa(newStart, newEnd);
        }
    }
}