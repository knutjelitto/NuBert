using Pliant.Grammars;

namespace Pliant.Builders
{
    public class LexerRuleModel : SymbolModel
    {
        public LexerRuleModel(ILexerRule value)
        {
            Value = value;
        }

        public override ISymbol Symbol => Value;

        public ILexerRule Value { get; }

        public override SymbolModelType ModelType => SymbolModelType.LexerRule;

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is LexerRuleModel lexerRuleModel && Value.Equals(lexerRuleModel.Value);
        }
    }
}