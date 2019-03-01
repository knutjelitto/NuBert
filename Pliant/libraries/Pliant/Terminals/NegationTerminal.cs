using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Terminals
{
    public sealed class NegationTerminal : Terminal
    {
        public NegationTerminal(Terminal innerTerminal)
        {
            InnerTerminal = innerTerminal;
            this._hashCode = ComputeHashCode();
        }

        public Terminal InnerTerminal { get; }

        private static IReadOnlyList<Interval> CreateIntervals(Terminal innerTerminal)
        {
            var inverseIntervalList = new List<Interval>();
            var intervals = innerTerminal.GetIntervals();
            foreach (var interval in intervals)
            {
                var inverseIntervals = Interval.Inverse(interval);
                inverseIntervalList.AddRange(inverseIntervals);
            }

            return Interval.Group(inverseIntervalList);
        }

        public override bool IsMatch(char character)
        {
            return !InnerTerminal.IsMatch(character);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this._intervals ?? (this._intervals = CreateIntervals(InnerTerminal));
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is NegationTerminal other && 
                   InnerTerminal.Equals(other.InnerTerminal);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(InnerTerminal.GetHashCode(), "!".GetHashCode());
        }

        private readonly int _hashCode;

        private IReadOnlyList<Interval> _intervals;
    }
}