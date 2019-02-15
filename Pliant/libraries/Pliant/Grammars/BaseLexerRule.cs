using Pliant.Tokens;

namespace Pliant.Grammars
{
    public abstract class BaseLexerRule : ILexerRule
    {
        protected LexerRuleType _lexerRuleType;
        protected TokenType _tokenType;

        protected BaseLexerRule(LexerRuleType lexerRuleType, TokenType tokenType)
        {
            this._lexerRuleType = lexerRuleType;
            this._tokenType = tokenType;
        }

        public LexerRuleType LexerRuleType => this._lexerRuleType;

        public SymbolType SymbolType => SymbolType.LexerRule;

        public TokenType TokenType => this._tokenType;

        public abstract bool CanApply(char c);
    }
}