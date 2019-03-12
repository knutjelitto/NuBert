using System.Collections.Generic;

namespace Pliant.Terminals
{
    public sealed class DigitTerminal : AtomTerminal
    {
        public static readonly DigitTerminal Instance = new DigitTerminal();
        public static readonly NegationTerminal NotIt = new NegationTerminal(Instance);

        private DigitTerminal()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is DigitTerminal;
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
            return '0' <= character && character <= '9';
        }

        public override string ToString()
        {
            return "[0-9]";
        }

        private static readonly Interval[] Intervals = {new Interval('0', '9')};
    }
}