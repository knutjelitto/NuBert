using System;
using System.Collections.Generic;
using System.Text;

namespace Lingu.Terminals
{
    public class NotTerminal : Terminal
    {
        private readonly Terminal negate;

        public NotTerminal(Terminal negate)
        {
            this.negate = negate;
        }

        public override bool Match(char character)
        {
            return this.negate.NotMatch(character);
        }

        public override bool NotMatch(char character)
        {
            return this.negate.Match(character);
        }

        public override string ToString()
        {
            return $"!({this.negate})";
        }
    }
}
