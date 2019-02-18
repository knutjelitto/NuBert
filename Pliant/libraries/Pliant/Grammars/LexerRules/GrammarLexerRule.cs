using Pliant.Runtime;
using Pliant.Tokens;

namespace Pliant.Grammars
{
    public class GrammarLexerRule : LexerRule
    {
        public GrammarLexerRule(string tokenType, IGrammar grammar)
            : this(new TokenType(tokenType), grammar)
        {
        }

        public GrammarLexerRule(TokenType tokenType, IGrammar grammar)
            : base(GrammarLexerRuleType, tokenType)
        {
            Grammar = grammar;
        }

        public IGrammar Grammar { get; }

        public override bool CanApply(char c)
        {
            // this is the best I could come up with without copying the initialization and reduction code necessary to 
            // determine if the lexer rules are indeed start rules
            for (var i = 0; i < Grammar.LexerRules.Count; i++)
            {
                var lexerRule = Grammar.LexerRules[i];
                if (lexerRule.CanApply(c))
                {
                    return true;
                }
            }

            return false;
        }

        public override ILexeme CreateLexeme(int position)
        {
            return new ParseEngineLexeme(this);
        }

        public override string ToString()
        {
            return TokenType.Id;
        }

        public static readonly LexerRuleType GrammarLexerRuleType = new LexerRuleType("Grammar");
    }
}