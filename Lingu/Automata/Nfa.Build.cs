using System;

namespace Lingu.Automata
{
    public partial class Nfa
    {
        public static Nfa operator +(Nfa n1, Nfa n2)
        {
            return n1.Concat(n2);
        }

        public static Nfa operator |(Nfa n1, Nfa n2)
        {
            return n1.Or(n2);
        }

        public Nfa Plus => this.Plus();

        public Nfa Star => this.Star();

        public Nfa Opt => this.Optional();

        public static explicit operator Nfa((char, char) pair)
        {
            return NfaBuilder.From(pair.Item1, pair.Item2);
        }

        public static explicit operator Nfa(char ch)
        {
            return NfaBuilder.From(ch);
        }
    }
}