using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class LexerRuleModel : SymbolModel
    {
        public LexerRuleModel(LexerRule value)
        {
            Value = value;
        }

        public override Symbol Symbol => Value;

        public LexerRule Value { get; }

        public override bool Equals(object obj)
        {
            return obj is LexerRuleModel other && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}