using Lingu.Terminals;

namespace Lingu.Automata
{
    public static class NfaSource
    {
        private static Nfa Single(Terminal terminal)
        {
            var start = new NfaState();
            var end = new NfaState();

            start.Add(terminal, end);

            return new Nfa(start, end);
        }

        public static Nfa From(char ch)
        {
            return Single(new SingleTerminal(ch));
        }

        public static Nfa From(char first, char last)
        {
            return Single(new RangeTerminal(first, last));
        }

    }
}
