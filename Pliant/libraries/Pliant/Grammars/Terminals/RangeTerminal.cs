﻿using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class RangeTerminal : Terminal
    {
        public RangeTerminal(char start, char end)
        {
            Start = start;
            End = end;
        }

        public char End { get; }
        public char Start { get; }

        public override bool Equals(object obj)
        {
            return obj is RangeTerminal other && Start.Equals(other.Start) && End.Equals(other.End);
        }

        public override int GetHashCode()
        {
            return (Start, End).GetHashCode();
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return this._intervals ?? (this._intervals = new[] {new Interval(Start, End)});
        }

        public override bool IsMatch(char character)
        {
            return Start <= character && character <= End;
        }

        public override string ToString()
        {
            return $"[{Start}-{End}]";
        }

        private Interval[] _intervals;
    }
}