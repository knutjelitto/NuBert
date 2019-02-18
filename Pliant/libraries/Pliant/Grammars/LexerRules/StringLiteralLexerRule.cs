using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class StringLiteralLexerRule : LexerRule //, IStringLiteralLexerRule
    {
        public StringLiteralLexerRule(string literal, TokenType tokenType)
            : base(StringLiteralLexerRuleType, tokenType)
        {
            Literal = literal;
        }

        public StringLiteralLexerRule(string literal)
            : this(literal, new TokenType(literal))
        {
        }

        public string Literal { get; }

        public override bool CanApply(char c)
        {
            return Literal.Length != 0 && Literal[0].Equals(c);
        }

        public override ILexeme CreateLexeme(int position)
        {
            return new StringLiteralLexeme(this, position);
        }

        public override bool Equals(object obj)
        {
            return obj is StringLiteralLexerRule other &&
                   LexerRuleType.Equals(other.LexerRuleType) &&
                   Literal.Equals(other.Literal);
        }

        public override int GetHashCode()
        {
            return (StringLiteralLexerRuleType, TokenType, Literal).GetHashCode();
        }

        public override string ToString()
        {
            return Literal;
        }

        public static readonly LexerRuleType StringLiteralLexerRuleType = new LexerRuleType("StringLiteral");
    }
}