using Lingu.Terminals;

namespace Lingu.Automata
{
    public static class NfaBuilder
    {
        private static Nfa Single(Terminal terminal)
        {
            var start = new NfaState();
            var end = new NfaState();

            start.Add(terminal, end);

            return new Nfa(start, end);
        }

        public static Nfa Any()
        {
            return Single(Terminal.From(UnicodeSets.Any));
        }


        public static Nfa From(char ch)
        {
            return Single(Terminal.From(ch));
        }

        public static Nfa From(char first, char last)
        {
            return Single(Terminal.From(first, last));
        }

        public static Nfa Optional(this Nfa nfa)
        {
            var clone = nfa.Clone();

            clone.Start.Add(clone.End);

            return clone;
        }

        public static Nfa Star(this Nfa nfa)
        {
            var clone = nfa.Clone();

            clone.Start.Add(clone.End);
            clone.End.Add(clone.Start);

            return clone;
        }


        public static Nfa Plus(this Nfa nfa)
        {
            var clone = nfa.Clone();

            clone.End.Add(clone.Start);

            return clone;
        }

        public static Nfa Or(this Nfa nfa, Nfa other)
        {
            var first = nfa.Clone();
            var second = other.Clone();
            var newEnd = new NfaState();

            first.Start.Add(second.Start);

            first.End.Add(newEnd);
            second.End.Add(newEnd);


            return new Nfa(first.Start, newEnd);
        }

        public static Nfa Concat(this Nfa nfa, Nfa concat)
        {
            var first = nfa.Clone();

            first.End.Add(concat.Start);

            return new Nfa(first.Start, concat.End);
        }
    }
}
