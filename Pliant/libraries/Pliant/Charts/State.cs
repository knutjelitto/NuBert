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

        public NonTerminal LeftHandSide => DottedRule.Production.LeftHandSide;

        public IForestNode ParseNode { get; protected set; }

        public abstract bool Enqueue(EarleySet set);
    }
}