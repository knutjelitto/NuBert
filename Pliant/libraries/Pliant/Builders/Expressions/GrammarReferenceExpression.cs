using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class GrammarReferenceExpression : BaseExpression
    {
        public GrammarReferenceExpression(IGrammar grammar)
        {
            GrammarReferenceModel = new GrammarReferenceModel(grammar);
        }

        public GrammarReferenceModel GrammarReferenceModel { get; }
    }
}