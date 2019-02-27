﻿using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    static class NormalStateHashCodeAlgorithm
    {
        public static int Compute(DottedRule dottedRule, int origin)
        {
            return HashCode.Compute(
                dottedRule.Dot.GetHashCode(),
                origin.GetHashCode(),
                dottedRule.Production.GetHashCode());
        }
    }
}
