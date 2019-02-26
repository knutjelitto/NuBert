using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public sealed class AlterationModel
    {
        public AlterationModel()
            : this(Enumerable.Empty<SymbolModel>())
        {
        }

        public AlterationModel(IEnumerable<SymbolModel> symbols)
        {
            this.symbols = new List<SymbolModel>(symbols);
        }

        public IReadOnlyList<SymbolModel> Symbols => this.symbols;

        public void AddSymbol(SymbolModel symbol)
        {
            this.symbols.Add(symbol);
        }

        public override string ToString()
        {
            return string.Join(" ", Symbols);
        }

        private readonly List<SymbolModel> symbols;
    }
}