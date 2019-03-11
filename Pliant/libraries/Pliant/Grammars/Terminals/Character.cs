using System.Collections.Generic;
using Pliant.Grammars;

namespace Pliant.Terminals
{
    public abstract class Character : Terminal
    {
        public abstract IReadOnlyList<Interval> GetIntervals();
    }
}