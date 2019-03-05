using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tokens;

namespace Pliant.LexerRules
{
    public class GrammarLexerRule : LexerRule
    {
        public GrammarLexerRule(string tokenType, Grammar grammar)
            : this(new TokenType(tokenType), grammar)
        {
        }

        public GrammarLexerRule(TokenType tokenType, Grammar grammar)
            : base(tokenType)
        {
            Grammar = grammar;
        }

        public Grammar Grammar { get; }

        public override bool CanApply(char c)
        {
            // this is the best I could come up with without copying the initialization and reduction code necessary to 
            // determine if the lexer rules are indeed start rules
            foreach (var lexerRule in Grammar.LexerRules)
            {
                if (lexerRule.CanApply(c))
                {
                    return true;
                }
            }

            return false;
        }

        public override Lexeme CreateLexeme(int position)
        {
            return new ParseEngineLexeme(this);
        }

        public override string ToString()
        {
            return TokenType.Id;
        }
    }
}