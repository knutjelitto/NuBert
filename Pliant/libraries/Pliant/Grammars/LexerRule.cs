using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class LexerRule : Symbol
    {
        protected LexerRule(TokenClass tokenType)
        {
            TokenType = tokenType;
        }

        public TokenClass TokenType { get; }

        public abstract bool CanApply(char c);

        public abstract Lexeme CreateLexeme(int position);
    }
}