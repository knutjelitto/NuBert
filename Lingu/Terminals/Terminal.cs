using System;

namespace Lingu.Terminals
{
    public abstract class Terminal : IComparable
    {
        public abstract bool Match(char character);
        public abstract bool NotMatch(char character);

        public NotTerminal Not()
        {
            return new NotTerminal(this);
        }

        public abstract int CompareTo(object obj);
    }
}