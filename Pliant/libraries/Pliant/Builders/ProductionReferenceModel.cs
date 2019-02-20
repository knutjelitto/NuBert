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

        public override Symbol Symbol => Reference;

        private NonTerminal Reference { get; }
    }
}