using Pliant.Grammars;

namespace Pliant.Builders
{
    public sealed class LexerRuleModel : SymbolModel
    {
        public LexerRuleModel(Lexer lexerRule)
            : base(lexerRule)
        {
        }

        public Lexer LexerRule => (Lexer) Symbol;

        public override bool Equals(object obj)
        {
            return obj is LexerRuleModel other && LexerRule.Equals(other.LexerRule);
        }

        public override int GetHashCode()
        {
            return LexerRule.GetHashCode();
        }
    }
}