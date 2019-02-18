using System.Collections.Generic;

namespace Pliant.Grammars
{
    public sealed class Optional : Grouping
    {
        public Optional(IReadOnlyList<Symbol> items)
            : base(items)
        {
        }
    }
}