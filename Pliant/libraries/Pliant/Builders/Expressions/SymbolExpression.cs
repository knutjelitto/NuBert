namespace Pliant.Builders.Expressions
{
    public class SymbolExpression : BaseExpression
    {
        public SymbolExpression(SymbolModel symbolModel)
        {
            SymbolModel = symbolModel;
        }

        public SymbolModel SymbolModel { get; }
    }
}