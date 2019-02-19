using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IProduction
    {
        NonTerminal LeftHandSide { get; }

        IReadOnlyList<ISymbol> RightHandSide { get; }

        bool IsEmpty { get; }
    }
}