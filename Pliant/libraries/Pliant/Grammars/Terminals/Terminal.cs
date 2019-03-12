#if false
using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Terminals
{
    public abstract class Terminal : Symbol
    {
        public abstract bool IsMatch(char character);

        public abstract IReadOnlyList<Interval> GetIntervals();
    }
}
#endif