using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class LexerRule : Symbol
    {
        protected LexerRule(TokenClass tokenClass)
        {
            TokenClass = tokenClass;
        }

        public TokenClass TokenClass { get; }

        public abstract bool CanApply(char c);

        public abstract Lexeme CreateLexeme(int position);
    }
}