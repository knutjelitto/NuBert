using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public sealed class SymbolForestNode : InternalForestNode, ISymbolForestNode
    {
        public SymbolForestNode(ISymbol symbol, int origin, int location, params IAndForestNode[] children)
            : base(origin, location, children)
        {
            Symbol = symbol;
            this._hashCode = ComputeHashCode();
        }

        public override ForestNodeType NodeType => ForestNodeType.Symbol;

        public ISymbol Symbol { get; }

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
            return this._hashCode;
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int) NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Symbol.GetHashCode());
        }

        private readonly int _hashCode;
    }
}