using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class LexerRule : Symbol
    {
        protected LexerRule(LexerRuleType lexerRuleType, TokenType tokenType)
        {
            LexerRuleType = lexerRuleType;
            TokenType = tokenType;
        }

        public LexerRuleType LexerRuleType { get; }

        public TokenType TokenType { get; }

        public abstract bool CanApply(char c);

        public abstract ILexeme CreateLexeme(int position);
    }
}