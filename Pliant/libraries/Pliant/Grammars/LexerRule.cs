using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class LexerRule : Symbol
    {
        protected LexerRule(TokenName tokenName)
        {
            TokenName = tokenName;
        }

        public TokenName TokenName { get; }

        public abstract bool CanApply(char c);

        public abstract Lexeme CreateLexeme(int position);
    }
}