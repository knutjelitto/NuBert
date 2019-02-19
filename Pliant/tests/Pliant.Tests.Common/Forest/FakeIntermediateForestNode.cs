using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Forest
{
    public class FakeIntermediateForestNode : FakeInternalForestNode, IIntermediateForestNode
    {
        public FakeIntermediateForestNode(DottedRule dottedRule, int origin, int location, params IAndForestNode[] children)
            : base(origin, location, children)
        {
            DottedRule = dottedRule;
        }

        public DottedRule DottedRule { get; }

        public override ForestNodeType NodeType => ForestNodeType.Intermediate;
    }
}