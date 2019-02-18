using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class Grouping : Symbol
    {
        private readonly List<Symbol> _items;

        public IReadOnlyList<Symbol> Items => this._items;

        protected Grouping(IReadOnlyList<Symbol> items)
        {
            this._items = new List<Symbol>(items);
        }
    }
}