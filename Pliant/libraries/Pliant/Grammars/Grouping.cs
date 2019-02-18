using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class Grouping : Symbol
    {
        private readonly List<ISymbol> _items;

        public IReadOnlyList<ISymbol> Items => this._items;

        protected Grouping(IReadOnlyList<ISymbol> items)
        {
            this._items = new List<ISymbol>(items);
        }
    }
}