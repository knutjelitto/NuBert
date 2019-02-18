using System.Collections.Generic;

namespace Pliant.Grammars
{
    public sealed class Repetition : Grouping
    {
        public Repetition(IReadOnlyList<ISymbol> items)
            : base(items) { }
    }
}