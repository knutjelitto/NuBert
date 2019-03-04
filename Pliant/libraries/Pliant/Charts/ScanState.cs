using System;
using System.Collections.Generic;
using System.Text;
using Pliant.Dotted;
using Pliant.Forest;

namespace Pliant.Charts
{
    public class ScanState : NormalState
    {
        public ScanState(DottedRule dottedRule, int origin)
            : base(dottedRule, origin)
        {
        }

        public ScanState(DottedRule dottedRule, int origin, IForestNode parseNode)
            : base(dottedRule, origin, parseNode)
        {
        }
    }
}
