using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class ProductionReferenceModel : SymbolModel
    {
        public ProductionReferenceModel(IGrammar grammar)
            : base(grammar.Start)
        {
            Grammar = grammar;
        }

        public IGrammar Grammar { get; }
    }
}