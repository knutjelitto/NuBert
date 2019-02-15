using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class ProductionReferenceModel : SymbolModel
    {
        public ProductionReferenceModel(IGrammar grammar)
        {
            Grammar = grammar;
            Reference = grammar.Start;
        }

        public IGrammar Grammar { get; }

        public INonTerminal Reference { get; }

        public override SymbolModelType ModelType => SymbolModelType.Reference;

        public override ISymbol Symbol => Reference;
    }
}