using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface ISymbolForestNode : IInternalForestNode
    {
        Symbol Symbol { get; }
    }
}