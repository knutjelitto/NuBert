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
            End.AddTransistion(
                new NullNfaTransition(nfa.Start));
            return this;
        }

        public Nfa Kleene()
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddTransistion(new NullNfaTransition(Start));
            newStart.AddTransistion(new NullNfaTransition(newEnd));

            newEnd.AddTransistion(new NullNfaTransition(Start));
            End.AddTransistion(new NullNfaTransition(newEnd));

            return new Nfa(newStart, newEnd);
        }

        public Nfa Union(Nfa nfa)
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddTransistion(new NullNfaTransition(Start));
            newStart.AddTransistion(new NullNfaTransition(nfa.Start));

            End.AddTransistion(new NullNfaTransition(newEnd));
            nfa.End.AddTransistion(new NullNfaTransition(newEnd));

            return new Nfa(newStart, newEnd);
        }
    }
}