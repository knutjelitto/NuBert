using Pliant.Grammars;

namespace Pliant.Builders
{
    public class LexerRuleModel : SymbolModel
    {
        public LexerRuleModel(LexerRule value)
        {
            Value = value;
        }

        public override Symbol Symbol => Value;

        public LexerRule Value { get; }

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