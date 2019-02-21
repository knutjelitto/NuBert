using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Tests.Common.Forest
{
    public class FakeSymbolForestNode : FakeInternalForestNode, ISymbolForestNode
    {
        public FakeSymbolForestNode(Symbol symbol, int origin, int location, params IAndForestNode[] children) 
            : base(origin, location, children)
        {
            Symbol = symbol;
        }

        public FakeSymbolForestNode(string symbol, int origin, int location, params IAndForestNode[] children)
            : this(new NonTerminal(symbol), origin, location, children)
        {
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Symbol; }
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            throw new System.NotImplementedException();
        }

        public Symbol Symbol { get; private set; }
    }
}
