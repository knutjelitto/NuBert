using System;
using System.Diagnostics;

namespace Lingu.Terminals
{
    public class RangeTerminal : Terminal
    {
        private readonly char first;
        private readonly char last;

        public RangeTerminal(char first, char last)
        {
            Debug.Assert(first < last);

            this.first = first;
            this.last = last;
        }

        public override bool Match(char character)
        {
            return this.first <= character && character <= this.last;
        }

        public override bool NotMatch(char character)
        {
            return character < this.first || this.last < character;
        }

        public override string ToString()
        {
            return $"'{this.first}'-'{this.last}'";
        }
    }
}