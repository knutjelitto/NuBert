using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class NegationTerminal : Terminal
    {
        public Terminal InnerTerminal { get; private set; }

        private IReadOnlyList<Interval> _intervals;

        private readonly int _hashCode = 0;

        public NegationTerminal(Terminal innerTerminal)
        {
            InnerTerminal = innerTerminal;
            this._hashCode = ComputeHashCode();
        }

        private static IReadOnlyList<Interval> CreateIntervals(Terminal innerTerminal)
        {
            var inverseIntervalList = new List<Interval>();
            var intervals = innerTerminal.GetIntervals();
            for (var i = 0; i < intervals.Count; i++)
            {
                var inverseIntervals = Interval.Inverse(intervals[i]);
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
            if(this._intervals == null)
            {
                this._intervals = CreateIntervals(InnerTerminal);
            }

            return this._intervals;
        }

        public override int GetHashCode()
        {
            return this._hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var negationTerminal = obj as NegationTerminal;
            if (negationTerminal == null)
            {
                return false;
            }

            return negationTerminal.InnerTerminal.Equals(InnerTerminal);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(InnerTerminal.GetHashCode(), "!".GetHashCode());
        }
    }
}