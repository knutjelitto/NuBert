using Pliant.Dotted;
using Pliant.Forest;

namespace Pliant.Charts
{
    public class CompletedState : RuleState
    {
        public CompletedState(DottedRule dottedRule, int origin, IForestNode parseNode = null)
            : base(dottedRule, origin, parseNode)
        {
        }

        public void SetParseNode(IForestNode parseNode)
        {
            ParseNode = parseNode;
        }

        public override bool Enqueue(EarleySet set)
        {
            return set.Add(this);
        }
    }
}