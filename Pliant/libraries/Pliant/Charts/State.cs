using Pliant.Diagnostics;
using Pliant.Dotted;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public abstract class State
    {
        protected State(DottedRule dottedRule, int origin, IForestNode parseNode)
        {
            DottedRule = dottedRule;
            Origin = origin;
            ParseNode = parseNode;
        }

        public DottedRule DottedRule { get; }

        public int Origin { get; }

        public IForestNode ParseNode { get; set; }
    }
}