using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class LexerRule : Symbol
    {
        protected LexerRule(TokenType tokenType)
        {
            TokenType = tokenType;
        }

        public TokenType TokenType { get; }

        public abstract bool CanApply(char c);

        public abstract Lexeme CreateLexeme(int position);
    }
}