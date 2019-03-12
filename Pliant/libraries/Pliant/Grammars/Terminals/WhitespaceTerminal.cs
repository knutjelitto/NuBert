using System.Collections.Generic;

namespace Pliant.Terminals
{
    public class WhitespaceTerminal : AtomTerminal
    {
        public static readonly WhitespaceTerminal Instance = new WhitespaceTerminal();

        private WhitespaceTerminal()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is WhitespaceTerminal;
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
            return char.IsWhiteSpace(character);
        }

        public override string ToString()
        {
            return @"\s";
        }

        private static readonly Interval[] Intervals =
        {
            new Interval((char) 0x0009, (char) 0x000D),
            new Interval((char) 0x0020, (char) 0x0020),
            new Interval((char) 0x0085, (char) 0x0085),
            new Interval((char) 0x00A0, (char) 0x00A0),
            new Interval((char) 0x2000, (char) 0x200A),
            new Interval((char) 0x2028, (char) 0x2029),
            new Interval((char) 0x202f, (char) 0x202f),
            new Interval((char) 0x205f, (char) 0x205f),
            new Interval((char) 0x3000, (char) 0x3000)
        };
    }
}