using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public sealed class StringLiteralLexerRule : LexerRule
    {
        public StringLiteralLexerRule(string literal, TokenType tokenType)
            : base(tokenType)
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

        public override Lexeme CreateLexeme(int position)
        {
            return new StringLiteralLexeme(this, position);
        }

        public override bool Equals(object obj)
        {
            return obj is StringLiteralLexerRule other &&
                   Literal.Equals(other.Literal);
        }

        public override int GetHashCode()
        {
            return (TokenType, Literal).GetHashCode();
        }

        public override string ToString()
        {
            return Literal;
        }
    }
}