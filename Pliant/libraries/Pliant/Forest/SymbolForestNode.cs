using System.Diagnostics;
using Pliant.Grammars;

namespace Pliant.Forest
{
    public sealed class SymbolForestNode : InternalForestNode, ISymbolForestNode
    {
        public SymbolForestNode(Symbol symbol, int origin, int location, params AndForestNode[] children)
            : base(origin, location, children)
        {
            Debug.Assert(symbol is NonTerminal);

            Symbol = symbol;
            this.hashCode = (NodeType, Origin, Location, Symbol).GetHashCode();
        }

        public override ForestNodeType NodeType => ForestNodeType.Symbol;

        public Symbol Symbol { get; }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is ISymbolForestNode symbolNode && 
                   Location == symbolNode.Location && 
                   NodeType == symbolNode.NodeType && 
                   Origin == symbolNode.Origin && 
                   Symbol.Equals(symbolNode.Symbol);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        private readonly int hashCode;
    }
}