using Pliant.Grammars;

namespace Pliant.Builders
{
    public class ProductionReferenceModel : SymbolModel
    {
        public IGrammar Grammar { get; private set; }

        public INonTerminal Reference { get; private set; }

        public override SymbolModelType ModelType => SymbolModelType.Reference;

        public override ISymbol Symbol => Reference;

        public ProductionReferenceModel(IGrammar grammar)
        {
            Grammar = grammar;
            Reference = grammar.Start;
        }
    }
}
