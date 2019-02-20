using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public sealed class ProductionReferenceExpression : BaseExpression
    {
        public ProductionReferenceExpression(IGrammar grammar)
        {
            ProductionReferenceModel = new ProductionReferenceModel(grammar);
        }

        public ProductionReferenceModel ProductionReferenceModel { get; }
    }
}