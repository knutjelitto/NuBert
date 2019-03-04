using System;
using System.Collections.Generic;
using System.Text;
using Pliant.Dotted;
using Pliant.Forest;

namespace Pliant.Charts
{
    public class PredictState : NormalState
    {
        public PredictState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
        }

        public PredictState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : base(dottedRule, origin, parseNode)
        {
        }
    }
}
