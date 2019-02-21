using System.Collections.Generic;

namespace Pliant.Grammars
{
    public sealed class AnyTerminal : Terminal
    {
        public override bool Equals(object obj)
        {
            return obj is AnyTerminal;
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return Interval;
        }

        public override bool IsMatch(char character)
        {
            return true;
        }

        public override string ToString()
        {
            return ".";
        }

        private static readonly Interval[] Interval = {new Interval(char.MinValue, char.MaxValue)};
    }
}