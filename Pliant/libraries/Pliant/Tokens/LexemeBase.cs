using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Tokens
{
    public abstract class LexemeBase<TLexerRule> : Lexeme
        where TLexerRule : LexerRule
    {
        protected LexemeBase(TLexerRule lexerRule, int position)
            : base(lexerRule, position)
        {
            ConcreteLexerRule = lexerRule;
        }

        protected TLexerRule ConcreteLexerRule { get; private set; }
    }
}