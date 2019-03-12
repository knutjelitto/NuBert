using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class LexerRule : Terminal
    {
        protected LexerRule(TokenName tokenName)
        {
            TokenName = tokenName;
        }

        public TokenName TokenName { get; }

        public abstract Lexeme CreateLexeme(int position);
    }
}