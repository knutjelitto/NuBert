using System;
using System.Diagnostics;

namespace Lingu.Terminals
{
    public class RangeTerminal : Terminal, IComparable<RangeTerminal>
    {
        private readonly char first;
        private readonly char last;

        public RangeTerminal(char first, char last)
        {
            Debug.Assert(first <= last);

            this.first = first;
            this.last = last;
        }

        public RangeTerminal(char first)
            : this(first, first)
        {
        }

        public override bool Match(char character)
        {
            return this.first <= character && character <= this.last;
        }

        public override bool NotMatch(char character)
        {
            return character < this.first || this.last < character;
        }

        public override bool Equals(object obj)
        {
            return obj is RangeTerminal other && this.first == other.first && this.last == other.last;
        }

        public override int GetHashCode()
        {
            return (this.first, this.last).GetHashCode();
        }

        public override string ToString()
        {
            if (this.first == this.last)
            {
                return $"'{this.first}'";
            }
            return $"'{this.first}'-'{this.last}'";
        }

        public override int CompareTo(object obj)
        {
            return CompareTo(obj as RangeTerminal);
        }

        public int CompareTo(RangeTerminal other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }
            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            var firstComparison = this.first.CompareTo(other.first);
            return firstComparison != 0
                       ? firstComparison
                       : this.last.CompareTo(other.last);
        }
    }
}