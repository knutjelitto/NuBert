using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class DigitTerminal : Terminal
    {
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
            return _intervals;
        }

        public override bool IsMatch(char character)
        {
            return char.IsDigit(character);
        }

        public override string ToString()
        {
            return "[0-9]";
        }

        private static readonly Interval[] _intervals = {new Interval('0', '9')};
    }
}