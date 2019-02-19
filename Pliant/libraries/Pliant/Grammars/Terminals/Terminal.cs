using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class Terminal : Symbol
    {
        protected Terminal()
        {
        }

        public abstract bool IsMatch(char character);

        public abstract IReadOnlyList<Interval> GetIntervals();
    }
}