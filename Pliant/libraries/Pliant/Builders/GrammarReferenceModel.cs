using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class GrammarReferenceModel : SymbolModel
    {
        public GrammarReferenceModel(IGrammar grammar)
            : base(grammar.Start)
        {
            Grammar = grammar;
        }

        public IGrammar Grammar { get; }
    }
}