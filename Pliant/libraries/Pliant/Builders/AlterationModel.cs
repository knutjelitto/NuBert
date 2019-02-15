using System.Collections.Generic;

namespace Pliant.Builders
{
    public class AlterationModel
    {
        public AlterationModel()
        {
            Symbols = new List<SymbolModel>();
        }

        public AlterationModel(IEnumerable<SymbolModel> symbols)
        {
            Symbols = new List<SymbolModel>(symbols);
        }

        public IList<SymbolModel> Symbols { get; }
    }
}