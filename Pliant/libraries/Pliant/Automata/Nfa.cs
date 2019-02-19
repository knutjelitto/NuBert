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

        public Nfa Concatenation(Nfa nfa)
        {
            End.AddTransition(
                new NullNfaTransition(nfa.Start));
            return this;
        }

        public Nfa Kleene()
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddTransition(new NullNfaTransition(Start));
            newStart.AddTransition(new NullNfaTransition(newEnd));

            newEnd.AddTransition(new NullNfaTransition(Start));
            End.AddTransition(new NullNfaTransition(newEnd));

            return new Nfa(newStart, newEnd);
        }

        public Nfa Union(Nfa nfa)
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddTransition(new NullNfaTransition(Start));
            newStart.AddTransition(new NullNfaTransition(nfa.Start));

            End.AddTransition(new NullNfaTransition(newEnd));
            nfa.End.AddTransition(new NullNfaTransition(newEnd));

            return new Nfa(newStart, newEnd);
        }
    }
}