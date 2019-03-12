using System.Collections.Generic;

namespace Pliant.Terminals
{
    public sealed class AsciiLetterTerminal : AtomTerminal
    {
        public static readonly AsciiLetterTerminal Instance = new AsciiLetterTerminal();

        private AsciiLetterTerminal()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is AsciiLetterTerminal;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return Intervals;
        }

        public override bool CanApply(char character)
        {
            return 'a' <= character && character <= 'z' || 'A' <= character && character <= 'Z';
        }

        public override string ToString()
        {
            return "[a-zA-z]";
        }

        private static readonly Interval[] Intervals = { new Interval('a', 'z'), new Interval('A', 'Z') };


    }
}
