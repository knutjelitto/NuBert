using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Terminals
{
    public sealed class AnyTerminal : AtomTerminal
    {
        public static readonly AnyTerminal Instance = new AnyTerminal();

        private AnyTerminal()
        {
        }

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

        public override bool CanApply(char character)
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