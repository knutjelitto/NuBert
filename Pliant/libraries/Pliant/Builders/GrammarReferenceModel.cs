using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class GrammarReferenceModel : SymbolModel
    {
        public GrammarReferenceModel(Grammar grammar)
            : base(grammar.Start)
        {
            Grammar = grammar;
        }

        public Grammar Grammar { get; }
    }
}