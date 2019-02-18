﻿using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class Terminal : Symbol
    {
        public abstract bool IsMatch(char character);

        public abstract IReadOnlyList<Interval> GetIntervals();
    }
}