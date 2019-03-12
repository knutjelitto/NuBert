using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Terminals
{
    public sealed class NegationTerminal : AtomTerminal
    {
        public NegationTerminal(AtomTerminal innerTerminal)
        {
            InnerTerminal = innerTerminal;
            this.hashCode = ("!", InnerTerminal).GetHashCode();
        }

        public AtomTerminal InnerTerminal { get; }

        private static IReadOnlyList<Interval> CreateIntervals(AtomTerminal innerTerminal)
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

        public override bool CanApply(char character)
        {
            return !InnerTerminal.CanApply(character);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this.intervals ?? (this.intervals = CreateIntervals(InnerTerminal));
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is NegationTerminal other && 
                   InnerTerminal.Equals(other.InnerTerminal);
        }

        private readonly int hashCode;

        private IReadOnlyList<Interval> intervals;
    }
}