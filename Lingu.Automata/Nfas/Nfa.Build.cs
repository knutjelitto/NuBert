namespace Lingu.Automata
{
    public partial class Nfa
    {
        public static Nfa operator +(Nfa n1, Nfa n2)
        {
            return NfaBuilder.Concat(n1, n2);
        }

        public static Nfa operator |(Nfa n1, Nfa n2)
        {
            return NfaBuilder.Or(n1, n2);
        }

        public Nfa Plus => NfaBuilder.Plus(this);

        public Nfa Star => NfaBuilder.Star(this);

        public Nfa Opt => NfaBuilder.Optional(this);

        public static Nfa Any => NfaBuilder.Dot;

        public static implicit operator Nfa((char first, char last) range)
        {
            return NfaBuilder.From(range.first, range.last);
        }

        public static implicit operator Nfa(char @char)
        {
            return NfaBuilder.From(@char);
        }

        public static implicit operator Nfa(string sequence)
        {
            return NfaBuilder.From(sequence);
        }

        public static explicit operator Nfa(Atom terminal)
        {
            return NfaBuilder.Single(terminal);
        }

        private static class NfaBuilder
        {
            public static Nfa Single(Atom terminal)
            {
                var start = new NfaState();
                var end = new NfaState();

                start.Add(terminal, end);

                return new Nfa(start, end);
            }

            public static Nfa Dot => Single(Atom.From(UnicodeSets.Any));

            public static Nfa From(char ch)
            {
                return Single(Atom.From(ch));
            }

            public static Nfa From(char first, char last)
            {
                return Single(Atom.From(first, last));
            }

            public static Nfa From(string sequence)
            {
                var start = new NfaState();
                var current = start;
                var next = (NfaState) null;

                foreach (var ch in sequence)
                {
                    next = new NfaState();
                    current.Add(Atom.From(ch), next);
                    current = next;
                }

                return new Nfa(start, next);
            }

            public static Nfa Optional(Nfa nfa)
            {
                var clone = nfa.Clone();

                clone.Start.Add(clone.End);

                return clone;
            }

            public static Nfa Star(Nfa nfa)
            {
                var clone = nfa.Clone();

                clone.Start.Add(clone.End);
                clone.End.Add(clone.Start);

                return clone;
            }

            public static Nfa Plus(Nfa nfa)
            {
                var clone = nfa.Clone();

                clone.End.Add(clone.Start);

                return clone;
            }

            public static Nfa Or(Nfa nfa, Nfa other)
            {
                var first = nfa.Clone();
                var second = other.Clone();
                var newEnd = new NfaState();

                first.Start.Add(second.Start);

                first.End.Add(newEnd);
                second.End.Add(newEnd);


                return new Nfa(first.Start, newEnd);
            }

            public static Nfa Concat(Nfa nfa, Nfa concat)
            {
                var first = nfa.Clone();

                first.End.Add(concat.Start);

                return new Nfa(first.Start, concat.End);
            }
        }

    }
}