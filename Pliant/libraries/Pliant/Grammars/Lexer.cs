using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public abstract class Lexer : Symbol
    {
        protected Lexer(TokenType tokenType)
        {
            TokenType = tokenType;
        }

        public TokenType TokenType { get; }

        public abstract bool CanApply(char c);

        public abstract Lexeme CreateLexeme(int position);
    }
}