using Pliant.Grammars;

namespace Pliant.Forest
{
    public sealed class SymbolForestNode : InternalForestNode, ISymbolForestNode
    {
        public SymbolForestNode(NonTerminal symbol, int origin, int location, params AndForestNode[] children)
            : base(origin, location, children)
        {
            Symbol = symbol;
            this.hashCode = (Origin, Location, Symbol).GetHashCode();
        }

        public Symbol Symbol { get; }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            return obj is ISymbolForestNode symbolNode &&
                   Origin == symbolNode.Origin &&
                   Location == symbolNode.Location &&
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