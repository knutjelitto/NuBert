using Pliant.Forest;

namespace Pliant.Charts
{
    public class CompletedState : EarleyItem
    {
        public CompletedState(DottedRule dottedRule, int origin, IForestNode parseNode)
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