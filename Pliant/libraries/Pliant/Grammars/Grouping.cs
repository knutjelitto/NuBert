using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class Grouping : IGrouping
    {
        private readonly List<ISymbol> _items;

        public IReadOnlyList<ISymbol> Items => this._items;

        public Grouping(IReadOnlyList<ISymbol> items)
        {
            this._items = new List<ISymbol>(items);
        }

        public virtual SymbolType SymbolType => SymbolType.Grouping;
    }
}